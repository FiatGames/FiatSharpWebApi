# FiatSharpWebApi

This is a template for creating a web api that Fiat can talk to.  Your game should already implement `IFiatGame` in order to use this template.

You must make changes to two files to implement your game:
### Global.asax.cs
Switch this line: `ConverterHelper.AddConverters<Settings, State, Move>(GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.Converters);` to be for your game types instead of for the TicTacToe example.

### Controllers/FiatGameController.cs
Make this controller point to your game types instead of for the TicTacToe example.
