echo 'Generating methods...'
dotnet fsi MethodsGenerator.fsx

echo 'Generating types...'
dotnet fsi TypesGenerator.fsx