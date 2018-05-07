using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using FiatSharp.Examples.TicTacToe;
using FiatSharp.JsonConverters;
using FiatSharpWebApi.Controllers;
using Newtonsoft.Json.Converters;

namespace FiatSharpWebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {

            GlobalConfiguration.Configuration.MapHttpAttributeRoutes(new FiatSharp.RouteProviders.WebApiConfig.CustomDirectRouteProvider());

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //todo Make this point to your types
            ConverterHelper.AddConverters<Settings, State, Move>(GlobalConfiguration.Configuration.Formatters
                .JsonFormatter.SerializerSettings.Converters);

            //todo these are custom converters to handle the specific TicTacToe example
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.Converters
                .Add(new TupleConverter<Spot,Player?>());
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.Converters
                .Add(new EnumConverter());
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.Converters
                .Add(new Controllers.MoveConverter());
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.Converters
                .Add(new SettingsConverter());
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.Converters
                .Add(new StateConverter());

        }
    }
}
