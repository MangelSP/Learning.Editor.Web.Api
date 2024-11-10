using Jint;
using Learning.Editor.Web.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ClearScript.V8;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Diagnostics;
using IOFile = System.IO.File;


namespace Learning.Editor.Web.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExecuteCodeController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> ExecuteCode([FromBody] CodeRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Code) || string.IsNullOrWhiteSpace(request.Language))
            {
                return BadRequest(new CodeResponse { Output = "Code or language not provided." });
            }

            try
            {
                string output;

                switch (request.Language.ToLower())
                {
                    case "csharp":
                        output = await ExecuteCSharp(request.Code);
                        break;
                    case "javascript":
                        output = ExecuteJavaScript(request.Code);
                        break;
                    case "python":
                        output = ExecutePython(request.Code);
                        break;
                    case "java":
                        output = ExecuteJava(request.Code);
                        break;
                    case "c":
                        output = ExecuteCOrCpp(request.Code, isCpp: false);
                        break;
                    case "cpp":
                    case "c++":
                        output = ExecuteCOrCpp(request.Code, isCpp: true);
                        break;
                    default:
                        return BadRequest(new CodeResponse { Output = "Unsupported language." });
                }

                return Ok(new CodeResponse { Output = output });
            }
            catch (Exception ex)
            {
                return BadRequest(new CodeResponse { Output = ex.Message });
            }
        }

        private async Task<string> ExecuteCSharp(string code)
        {
            var scriptOptions = ScriptOptions.Default
                .WithReferences(typeof(object).Assembly, typeof(Console).Assembly)
                .WithImports("System");

            var result = await CSharpScript.EvaluateAsync(code, scriptOptions);
            return result?.ToString() ?? "No output";
        }

        private string ExecuteJavaScript(string code)
        {
            // Usar Jint para ejecutar JavaScript
            var engine = new Engine();
            var result = engine.Evaluate(code); // Ejecuta y obtiene el valor directamente
            return result?.ToString() ?? "No output";
        }

        private string ExecutePython(string code)
        {
            // Ejecuta el código Python usando un proceso externo
            var start = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"-c \"{code.Replace("\"", "\\\"")}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(start))
            {
                process.WaitForExit();
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();
                return string.IsNullOrEmpty(error) ? output : error;
            }
        }

        private string ExecuteJava(string code)
        {
            var tempFileName = Path.Combine(Path.GetTempPath(), "TempProgram.java");
            var tempClassName = "TempProgram";

            // Guardar el código en un archivo temporal
            IOFile.WriteAllText(tempFileName, code);

            try
            {
                // Compilar el archivo Java
                var compileProcess = new ProcessStartInfo
                {
                    FileName = "javac",
                    Arguments = tempFileName,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(compileProcess))
                {
                    process.WaitForExit();
                    var compileErrors = process.StandardError.ReadToEnd();
                    if (!string.IsNullOrEmpty(compileErrors))
                        return compileErrors;
                }

                // Ejecutar el archivo compilado
                var runProcess = new ProcessStartInfo
                {
                    FileName = "java",
                    Arguments = $"-cp {Path.GetTempPath()} {tempClassName}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(runProcess))
                {
                    process.WaitForExit();
                    var output = process.StandardOutput.ReadToEnd();
                    var errors = process.StandardError.ReadToEnd();
                    return string.IsNullOrEmpty(errors) ? output : errors;
                }
            }
            finally
            {
                // Limpiar archivos temporales
                IOFile.Delete(tempFileName);
                IOFile.Delete(Path.Combine(Path.GetTempPath(), $"{tempClassName}.class"));
            }
        }

        private string ExecuteCOrCpp(string code, bool isCpp)
        {
            var tempFileName = Path.Combine(Path.GetTempPath(), isCpp ? "TempProgram.cpp" : "TempProgram.c");
            var outputFileName = Path.Combine(Path.GetTempPath(), "TempProgram.exe");

            // Guardar el código en un archivo temporal
            IOFile.WriteAllText(tempFileName, code);

            try
            {
                // Compilar el archivo C/C++
                var compiler = isCpp ? "g++" : "gcc";
                var compileProcess = new ProcessStartInfo
                {
                    FileName = compiler,
                    Arguments = $"{tempFileName} -o {outputFileName}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(compileProcess))
                {
                    process.WaitForExit();
                    var compileErrors = process.StandardError.ReadToEnd();
                    if (!string.IsNullOrEmpty(compileErrors))
                        return compileErrors;
                }

                // Ejecutar el archivo compilado
                var runProcess = new ProcessStartInfo
                {
                    FileName = outputFileName,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(runProcess))
                {
                    process.WaitForExit();
                    var output = process.StandardOutput.ReadToEnd();
                    var errors = process.StandardError.ReadToEnd();
                    return string.IsNullOrEmpty(errors) ? output : errors;
                }
            }
            finally
            {
                // Limpiar archivos temporales
                IOFile.Delete(tempFileName);
                IOFile.Delete(outputFileName);
            }
        }


    }
}