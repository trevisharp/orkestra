# Orkestra

A technology to build your own technologies.

# Table of Contents

 - [Overview](#overview)
 - [How to install](#how-to-install)
 - [Learn by examples](#learn-by-examples)
 - [Versions](#versions)
 - [Next Features](#next-features)

# Overview

Orkestra is a C# Framework to create compilers and build complex project strcutures with a command line interface for them.

# How to install

```bash
dotnet new console # Create project
dotnet add package Orkestra # Install Orkestra
```

# Learn by examples

### Hello, Compilers

```cs
using Orkestra;

Tech.Run(args);

public class MyFirstCompiler : Compiler
{
    Key HELLO = "hello";
    Key COMPILERS = "compilers";

    Rule program;

    public MyFirstCompiler()
    {
        // Accept 'hello' or 'hello compilers'
        program = start(
            [ HELLO ],
            [ HELLO, COMPILERS ]
        );
    }
}
```
Success on test1.code:
```
hello compilers
```
Success on test2.code:
```
hello
```
And a error in test3.code:
```
compilers
```
See the console:
```
>> dotnet run run # First run is for C# second run is the parameter for the application
Build started...
Compiling 1 files...
Syntax error in C:\path\test3.code next to 'compilers' on line 1.
```

# Versions

### Orkestra v1.0.0 (Coming soon)

 - ![](https://img.shields.io/badge/new-green) Added Tree consumer system.

### Orkestra v0.8.0 (Coming soon)

 - ![](https://img.shields.io/badge/new-green) Auto key finder on rules.
 - ![](https://img.shields.io/badge/updated-blue) Improve Compiler Load and key/rule search process.
 - ![](https://img.shields.io/badge/new-green) Fast Compiler easy sintax added.

### Orkestra v0.7.0 (Coming soon)

 - ![](https://img.shields.io/badge/new-green) Default CLI, Project and Compiler for small projects.
 - ![](https://img.shields.io/badge/updated-blue) New application configuration and start workflow.
 - ![](https://img.shields.io/badge/updated-blue) Use Cache system by default.

### Orkestra v0.6.0 (Not published yet)

 - ![](https://img.shields.io/badge/new-green) Add Smart Autocomplete in extension generation.
 - ![](https://img.shields.io/badge/updated-blue) Improve error messages when using LR1 algorithm.
 - ![](https://img.shields.io/badge/removed-red) Replace the snippet contribute by autocomplete.

### Orkestra v0.5.0

 - ![](https://img.shields.io/badge/new-green) Improve key definition using intermediary objects. Now ruleA | ruleB, keyA | keyB is allowed.
 - ![](https://img.shields.io/badge/new-green) Improve auto convertion between rules and list expressions.
 - ![](https://img.shields.io/badge/removed-red) one and some rule functions in Compiler class.

### Orkestra v0.4.0

 - ![](https://img.shields.io/badge/new-green) Add Smart Snipets in extension generation.
 - ![](https://img.shields.io/badge/updated-blue) InstallExtension will not generate .vsix by default.
 - ![](https://img.shields.io/badge/updated-blue) GenerateExtension will generate .vsix now.
 - ![](https://img.shields.io/badge/new-green) Consider processings in extension generation.
 - ![](https://img.shields.io/badge/updated-blue) Add comments in extension generation.
 - ![](https://img.shields.io/badge/bug%20solved-orange) Bug fixes when create a language with low keywords.

### Orkestra v0.3.0

 - ![](https://img.shields.io/badge/new-green) Added default VSCode extension generation code with smart highligthing.
 - ![](https://img.shields.io/badge/updated-blue) Improve key and rule definition sintaxes.
 - ![](https://img.shields.io/badge/updated-blue) Improve Processing performance.
 - ![](https://img.shields.io/badge/bug%20solved-orange) Many bug fixes in Processing abstraction.

### Orkestra v0.2.0

 - ![](https://img.shields.io/badge/new-green) Added abstraction for IDE extension generation.
 - ![](https://img.shields.io/badge/updated-blue) Compiler is now non-abstract.
 - ![](https://img.shields.io/badge/bug%20solved-orange) Serveral bug fixes.

### Orkestra v0.1.0

 - ![](https://img.shields.io/badge/new-green) Lexical Analyzer System.
 - ![](https://img.shields.io/badge/new-green) Syntactical Analyzer System with LR1 implementation.
 - ![](https://img.shields.io/badge/new-green) Cache System.
 - ![](https://img.shields.io/badge/new-green) Project definition System.
 - ![](https://img.shields.io/badge/new-green) Command Line Interface definition System.

# Next Features

- Syntactic tree analyzes system.
