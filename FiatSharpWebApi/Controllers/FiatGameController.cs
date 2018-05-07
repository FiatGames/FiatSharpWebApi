using System;
using System.Collections.Generic;
using FiatSharp;
using FiatSharp.Controllers;
using FiatSharp.Examples.TicTacToe;
using Newtonsoft.Json;
using System.Linq;
using FiatSharp.Models;
using FiatSharp.Models.Http;
using Newtonsoft.Json.Linq;

namespace FiatSharpWebApi.Controllers
{
    //todo fix up these types to be for your game
    public class FiatGameController : FiatGameController<Settings,State,Move,Settings,State>
    {
        public override IFiatGame<Settings, State, Move, Settings, State> GetFiatGame() => new FiatGame();
        public override Settings GetDefaultSettings()
        {
            return base.GetDefaultSettings();
        }

        public override Settings PostAddPlayer(AddPlayerRequest<Settings> req)
        {
            var a = base.PostAddPlayer(req);
            return a;
        }

        public override InitialGameStateResult<Settings, State, Move> PostInitialGameState(Settings settings)
        {
            var a= base.PostInitialGameState(settings);
            return a;
        }

        public override GameState<State, Move> PostMakeMove(MakeMoveRequest<Settings, State, Move> req)
        {
            var a = base.PostMakeMove(req);
            return a;
        }
    }

    public class StateConverter : JsonConverter<State>
    {
        public override void WriteJson(JsonWriter writer, State value, JsonSerializer serializer)
        {
            JObject t;
            if (value.Winner.HasValue)
            {
                t = (JObject)JToken.FromObject(new
                {
                    board = value.Board.Select(kvp => new Tuple<Spot, Player?>(kvp.Key, kvp.Value)),
                    turn = value.Turn,
                    validMoves = value.ValidMoves(),
                    gameOver = new { tag = "Win", value = value.Winner }
                }, serializer);
            }
            else if (value.Tied)
            {
                t = (JObject)JToken.FromObject(new
                {
                    board = value.Board.Select(kvp => new Tuple<Spot, Player?>(kvp.Key, kvp.Value)),
                    turn = value.Turn,
                    validMoves = value.ValidMoves(),
                    gameOver = new { tag = "Draw"}
                }, serializer);
            }
            else
            {
                t = (JObject)JToken.FromObject(new
                {
                    board = value.Board.Select(kvp => new Tuple<Spot, Player?>(kvp.Key, kvp.Value)),
                    turn = value.Turn,
                    validMoves = value.ValidMoves(),
                    gameOver = (string)null
                }, serializer);
            }
            t.WriteTo(writer);
        }

        public override State ReadJson(JsonReader reader, Type objectType, State existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;

            JObject obj = JObject.Load(reader);
            var s = new State();
            s.Winner = obj["gameOver"].Type == JTokenType.Null
                ? null
                : obj["gameOver"]["tag"].ToObject<string>() == "Win"
                    ? obj["gameOver"]["value"].ToObject<Player?>(serializer)
                    : null;
            s.Tied = obj["gameOver"].Type != JTokenType.Null && obj["gameOver"]["tag"].ToObject<string>() == "Draw";
            s.Turn = obj["turn"].ToObject<Player>(serializer);
            s.Board = obj["board"].ToObject<IEnumerable<Tuple<Spot, Player?>>>(serializer)
                .ToDictionary(t => t.Item1, t => t.Item2);
            return s;
        }
    }

    public class EnumConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value.GetType().IsEnum)
            {
                JObject t = (JObject) JToken.FromObject(new
                {
                    tag = Enum.GetName(value.GetType(), value)
                });
                t.WriteTo(writer);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (!objectType.IsEnum) return existingValue;
            JObject obj = JObject.Load(reader);
            return (Spot)Enum.Parse(objectType, obj["tag"].ToObject<string>());
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsEnum;
        }
    }
    public class SettingsConverter : JsonConverter<Settings>
    {
        public override void WriteJson(JsonWriter writer, Settings value, JsonSerializer serializer)
        {
            JObject t = (JObject) JToken.FromObject(new
            {
                xPlayer = value.XPlayer,
                oPlayer = value.OPlayer,
                players = value.Players,
                timePerMove = value.TimePerMove
            }, serializer);
            t.WriteTo(writer);
        }

        public override Settings ReadJson(JsonReader reader, Type objectType, Settings existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            return new Settings
            {
                XPlayer = obj["xPlayer"].ToObject<FiatPlayer>(serializer),
                OPlayer = obj["oPlayer"].ToObject<FiatPlayer>(serializer),
                Players = obj["players"].ToObject<List<FiatPlayer>>(serializer),
                TimePerMove = obj["timePerMove"].ToObject<TimeSpan>()
            };
        }
    }

    public class MoveConverter : JsonConverter<Move>
    {
        public override void WriteJson(JsonWriter writer, Move value, JsonSerializer serializer)
        {
            var a = new Dictionary<Type, Action>
            {
                {
                    typeof(Forfeit), () =>
                    {
                        JObject t = (JObject) JToken.FromObject(new {tag = "Forfeit"},serializer);
                        t.WriteTo(writer);
                    }
                },
                {
                    typeof(PlaceMove), () =>
                    {
                        JObject t = (JObject) JToken.FromObject(new {tag = "PlaceMove", contents= ((PlaceMove)value).Spot },serializer);
                        t.WriteTo(writer);
                    }
                },
            };

            a[value.GetType()]();
        }

        public override Move ReadJson(JsonReader reader, Type objectType, Move existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;

            JObject obj = JObject.Load(reader);
            switch (obj["tag"].ToObject<string>())
            {
                case "Forfeit":
                    return new Forfeit();
                case "PlaceMove":
                    return new PlaceMove {Spot = obj["contents"].ToObject<Spot>(serializer)};
            }
            throw new Exception("Can't decode");
        }
    }
}
