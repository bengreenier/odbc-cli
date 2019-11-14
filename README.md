# ODBC Cli

A small ODBC driver test program

![CD Badge](https://github.com/bengreenier/odbc-cli/workflows/Master%20CD/badge.svg)

I needed a program that let me write quick test queries, executed said queries using an installed ODBC driver, and showed the results. This is that program.

## How to use

+ Get [The latest release](https://github.com/bengreenier/odbc-cli/releases/latest)
+ Modify (or create) `.config.json` next to the binary, with the following JSON content, populated for your use case:
```
{
 "connectionString": "Driver={Custom ODBC};Host=mysite.com;Port=1337;Ssl=1;AuthMech=0;"
}
```
+ Run `odbc-cli.exe`

## Contributing

Not at this time - sorry. This tool is not maintained, nor supported.

## License

MIT
