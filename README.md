# ODBC Cli

A small ODBC driver test program

![CD Badge](https://github.com/bengreenier/odbc-cli/workflows/Master%20CD/badge.svg)
![CI Badge](https://github.com/bengreenier/odbc-cli/workflows/Master%20CI/badge.svg)

I needed a program that let me write quick test queries, executed said queries using an installed ODBC driver, and showed the results. This is that program.

## How to use

- Get [The latest release](https://github.com/bengreenier/odbc-cli/releases/latest)
- Modify (or create) `.config.json` next to the binary, with the following JSON content, populated for your use case:

```
{
 "connectionString": "Driver={Custom ODBC};Host=mysite.com;Port=1337;Ssl=1;AuthMech=0;"
}
```

- Run `odbc-cli.exe`

## Arguments

The following sections describe the possible command line arguments.

### -

> Yes you read that correctly, simply `-` as a final argument.

This argument is used to indicate that empty stdin input should exit
the application. This affects both the REPL and piped input, but is most
useful in the piped scenario. For instance:

```
cat some_newline_delim_queries.txt | odbc-cli.exe -
```

Will cause the program to exit after all querys have run.

## Configuration

The following sections describe the json configuration values.

### connectionString

An ODBC connection string that we'll use to connect to the serivce.

### disableOutput

Disables printing results to stdout.

Note timing information is still printed for each query.

## Contributing

Not at this time - sorry. This tool is not maintained, nor supported.

## License

MIT
