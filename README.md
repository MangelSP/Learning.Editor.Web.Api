# Learning Code Editor API

This project is an API designed to execute code dynamically in multiple programming languages. Supported languages include **C#**, **JavaScript**, **Python**, **Java**, **C**, and **C++**. The API receives code from a frontend application (e.g., React) and returns the output or any errors.

## Technologies Used

- **ASP.NET Core**: Framework to build the API.
- **Microsoft.CodeAnalysis.CSharp.Scripting (Roslyn)**: To execute C# code.
- **Jint**: JavaScript engine for .NET.
- **Python**: Executed as an external process.
- **Java (JDK)**: Executed as an external process.
- **GCC/G++**: Compiler to execute C and C++ code.

## Prerequisites

### General

- .NET SDK 8.0 or higher
- Git (to clone the repository)

### System Dependencies

- **Python**: Install [Python](https://www.python.org/downloads/) and ensure it is added to the `PATH`.
- **Java (JDK)**: Install the [JDK](https://www.oracle.com/java/technologies/javase-jdk11-downloads.html) and ensure both `javac` and `java` are in the `PATH`.
- **GCC/G++**: Install [MinGW](http://www.mingw.org/) on Windows, or use GCC on Linux/Mac and add `gcc` and `g++` to the `PATH`.

### .NET Dependencies

Run the following command in the project root to install the required .NET dependencies:

```bash
dotnet add package Microsoft.CodeAnalysis.CSharp.Scripting
dotnet add package Jint

```
### 1. C# Example:

Request Json:
```
{
  "code": "return 5 + 5;",
  "language": "csharp"
}
```
Expected OutPut:
```
{
  "output": "10"
}
```

### 2. JavaScript Example:

Request Json:
```
{
  "code": "let x = 10; x * 2;",
  "language": "javascript"
}
```
Expected OutPut:
```
{
  "output": "20"
}
```

### 3. Python Example:

Request Json:
```
{
  "code": "print(5 + 5)",
  "language": "python"
}
```
Expected OutPut:
```
{
  "output": "10"
}
```

### 4. Java Example:

Request Json:
```
{
  "code": "public class TempProgram { public static void main(String[] args) { System.out.println(5 + 5); } }",
  "language": "java"
}
```
Expected OutPut:
```
{
  "output": "10"
}
```

### 5. C Example:

Request Json:
```
{
  "code": "#include <stdio.h>\nint main() { printf(\"%d\", 5 + 5); return 0; }",
  "language": "c"
}
```
Expected OutPut:
```
{
  "output": "10"
}
```

### 6. C++ Example:

Request Json:
```
{
  "code": "#include <iostream>\nint main() { std::cout << 5 + 5; return 0; }",
  "language": "cpp"
}
```
Expected OutPut:
```
{
  "output": "10"
}
```
