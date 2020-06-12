using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Prometheus;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.Issue.Example.Repository;
using Swashbuckle.Issue.Example.Service;
using Swashbuckle.Issue.Example.Web.Mvc.Filters;
using Swashbuckle.Issue.Example.Web.Mvc.Filters.ExceptionHandling;
using Swashbuckle.Issue.Example.Web.Swagger;
using ZNetCS.AspNetCore.Authentication.Basic;
using ZNetCS.AspNetCore.Authentication.Basic.Events;

[assembly: ApiConventionType(typeof(DefaultApiConventions))]
[assembly: AssemblyDescription("Demonstraction of swagger method categories all opening at once")]

namespace Swashbuckle.Issue.Example.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // ReSharper disable once UnusedMember.Global
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureAspNetService(services, Configuration);
            ConfigureJsonSerializer();
            ConfigureApplicationServices(services);
            ConfigureBasicAuthentication(services);
            ConfigureJwtBearerAuthentication(services);
            ConfigureDataProtection(services);
            ConfigureFluentValidation(services);
            ConfigureAutomapper(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // ReSharper disable once UnusedMember.Global
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IApiVersionDescriptionProvider apiVersionDescriptionProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                //Build a swagger endpoint for each discovered API version
                foreach (ApiVersionDescription description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                {
                    c.SwaggerEndpoint(
                        $"/swagger/{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant());
                }
            });
            //Order of these using statements matters as they set up the request filter order.
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            // Prometheus metrics - default endpoint is /metrics
            app.UseMetricServer();
            app.UseHttpMetrics();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/api/health");
                endpoints.MapControllers();
            });
        }

        private static void ConfigureJsonSerializer()
        {
            JsonConvert.DefaultSettings = () =>
            {
                JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
                {
                    Formatting = Formatting.None,
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
                jsonSerializerSettings.Converters.Add(
                    new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy() });
                jsonSerializerSettings.Converters.Add(
                    new IsoDateTimeConverter
                    {
                        DateTimeStyles = DateTimeStyles.AdjustToUniversal,
                        DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ssK"
                    });
                return jsonSerializerSettings;
            };
        }

        private static void AddSingleton<T>(IServiceCollection services, T instance)
        {
            services.Add(new ServiceDescriptor(typeof(T), instance));
        }

        private static void ConfigureAutomapper(IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
        }

        private static void ConfigureFluentValidation(IServiceCollection services)
        {
            services.AddSingleton<IValidator<ValueModel>, ValueModelValidator>();
        }

        /// <summary>
        ///     Adds HTTP headers to outbound requests issued from HTTPClient.
        /// </summary>
        /// <example>
        ///     <code>
        /// private void ConfigureSomeSdk(IServiceCollection services)
        /// {
        ///    AddSingleton&lt;ISomeSdkConfiguration&gt;(
        ///        services,
        ///         GetConfigurationSection&lt;SomeSdkConfiguration&gt;("ApiHosts:SaasE2E"));
        ///     services.AddHttpClient&lt;ISomeSdk, SomeSdk&gt;(SetupCommonHttpRequestHeaders);
        /// }
        /// </code>
        /// </example>
        private static void SetupCommonHttpRequestHeaders(HttpClient configuration)
        {
            configuration.DefaultRequestHeaders.Add("X-Application-Id", "swashbuckle-issue-example");
            string userAgent = configuration.DefaultRequestHeaders.UserAgent.ToString();
            string space = string.IsNullOrEmpty(userAgent) ? string.Empty : " ";
            userAgent += $"{space}swashbuckle-issue-example/{Assembly.GetEntryAssembly()?.GetName().Version}";
            configuration.DefaultRequestHeaders.Add("User-Agent", userAgent);
        }

        private static void ConfigureDataProtection(IServiceCollection services)
        {
            // Do not generate keys automatically since they should be the same
            // for each instance in a pool
            services.AddDataProtection()
                .SetApplicationName("swashbuckle-issue-example")
                .DisableAutomaticKeyGeneration();
        }

        private static void AddXmlComments(SwaggerGenOptions c, string fileName)
        {
            // Set the comments path for the Swagger JSON and UI.
            string basePath = AppContext.BaseDirectory;
            //TODO: Review to determine if documentation should be present in a production environment.
            string xmlPath = Path.Combine(AppContext.BaseDirectory, fileName);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath, true);
            }
        }

        private void ConfigureAspNetService(IServiceCollection services, IConfiguration configuration)
        {
            //Infrastructure services
            services.Configure<KestrelServerOptions>(options => { options.AddServerHeader = false; });
            services.AddHealthChecks();
            services.AddResponseCompression();
            // Add API controllers but no views or razor pages.
            services.AddControllers(
                    options =>
                    {
                        options.Filters.Add<RequestIdHeaderNormalizationFilter>();
                        options.Filters.Add<SerilogRequestPropertyPushingFilter>();
                        options.Filters.Add<SerilogUserPropertyPushingFilter>();
                        options.Filters.Add<RequestIdHeaderCopyToResponseFilter>();
                        // The exception filters seem to act like a stack with the last one pushed being the first one called.
                        options.Filters.Add<UnhandledExceptionLoggingFilter>();
                        options.Filters.Add<ExceptionToResultFilter>();
                    })
                // TODO - Configure Flurl to use an ISerializer with System.Text.Json https://flurl.dev/docs/configuration/#serializers
                .AddNewtonsoftJson(
                    opts =>
                    {
                        // camel case property names
                        opts.SerializerSettings.ContractResolver =
                            new CamelCasePropertyNamesContractResolver();
                        // Returns the name of the enumeration so that it is more "readable" in the response.
                        // However, the value is not translated nor intended to be displayed.
                        opts.SerializerSettings.Converters.Add(
                            new StringEnumConverter
                            {
                                NamingStrategy =
                                    new CamelCaseNamingStrategy()
                            });
                        opts.SerializerSettings.Converters.Add(
                            new IsoDateTimeConverter
                            {
                                DateTimeStyles =
                                    DateTimeStyles.AdjustToUniversal,
                                DateTimeFormat =
                                    "yyyy'-'MM'-'dd'T'HH':'mm':'ssK"
                            });
                        //Don't include null values in response
                        opts.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    })
                //Required to use the ApiController attribute
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddFluentValidation();
            services.AddLogging(
                loggingBuilder =>
                {
                    //Create Serilog logger from AppSettings.json properties.
                    var logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(configuration)
                        .CreateLogger();
                    loggingBuilder.AddSerilog(logger);
                });
            // TODO - Remove
            //Mapper reset is added because (for some unknown reason), when running `dotnet ef database drop` this is getting called twice and
            // results in the error: Application startup exception: System.InvalidOperationException: Mapper already initialized. You must call Initialize once per application domain/process
            // Mapper.Reset();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            ConfigureSwagger(services);
            services.AddHttpContextAccessor();
            services.AddMemoryCache();
            services.AddApiVersioning(
                options =>
                {
                    // reporting api versions will return the headers "api-supported-versions" and "api-deprecated-versions"
                    options.ReportApiVersions = true;
                });
            services.AddVersionedApiExplorer(
                options =>
                {
                    // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                    // note: the specified format code will format the version as "'v'major[.minor][-status]"
                    options.GroupNameFormat = "'v'VVV";

                    // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                    // can also be used to control the format of the API version in route templates
                    options.SubstituteApiVersionInUrl = true;
                });
        }

        private void ConfigureSwagger(IServiceCollection services)
        {
            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(
                c =>
                {
                    // ReSharper disable once CommentTypo
                    //See https://github.com/domaindrivendev/Swashbuckle.AspNetCore#assign-explicit-operationids
                    c.CustomOperationIds(
                        apiDesc =>
                            apiDesc.TryGetMethodInfo(out MethodInfo methodInfo)
                                ? methodInfo.Name
                                : null);
                    // Set the comments path for the Swagger JSON and UI.
                    //TODO: Review to determine if documentation should be present in a production environment.
                    AddXmlComments(c, "Swashbuckle.Issue.Example.Web.xml");
                    AddXmlComments(c, "Swashbuckle.Issue.Example.Service.xml");
                    AddXmlComments(c, "Swashbuckle.Issue.Example.Repository.xml");
                    AddXmlComments(c, "Swashbuckle.Issue.Example.HttpClient.xml");
                    c.AddSecurityDefinition(
                        BasicAuthenticationDefaults.AuthenticationScheme,
                        new OpenApiBasicAuthSecurityScheme());
                    c.OperationFilter<SwashbuckleSecurityRequirementsOperationFilter>();
                    c.AddFluentValidationRules();

                    // For Api Versions
                    // Resolve the temporary IApiVersionDescriptionProvider service
                    IApiVersionDescriptionProvider provider = services.BuildServiceProvider()
                        .GetRequiredService<IApiVersionDescriptionProvider>();

                    // Add a swagger document for each discovered API version
                    Assembly assembly = GetType().Assembly;
                    foreach (ApiVersionDescription description in provider.ApiVersionDescriptions)
                    {
                        string version = description.ApiVersion.ToString();
                        string desc = description.IsDeprecated
                            ? $"{assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description} - DEPRECATED"
                            : assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description ?? string.Empty;
                        OpenApiInfo info = new OpenApiInfo
                        {
                            Title = $"Swashbuckle Issue Example V{description.ApiVersion}",
                            Version = version,
                            Description = desc,
                            Contact = new OpenApiContact
                            {
                                Name = "Richard Collette", Email = "maintainer@acme.com"
                            }
                        };
                        c.SwaggerDoc(description.GroupName, info);
                    }
                });
            services.AddSwaggerGenNewtonsoftSupport(); // explicit opt-in - needs to be placed after AddSwaggerGen()
            services.ConfigureSwaggerGen(
                options => { options.CustomSchemaIds(x => x.FullName); });
        }

        private void ConfigureBasicAuthentication(IServiceCollection services)
        {
            string userName = Configuration.GetValue<string>("Authentication:BasicAuth:Username");
            string password = Configuration.GetValue<string>("Authentication:BasicAuth:Password");
            services
                .AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
                .AddBasicAuthentication(
                    options =>
                    {
                        options.Realm = "todo-basic-auth-realm";
                        options.AjaxRequestOptions.SuppressWwwAuthenticateHeader = true;
                        options.Events = new BasicAuthenticationEvents
                        {
                            OnValidatePrincipal = context =>
                            {
                                // This is a simple basic authentication function that validates
                                // a single username and password as configured in appsettings.json
                                if (context.UserName != userName || context.Password != password)
                                {
                                    return Task.CompletedTask;
                                }

                                List<Claim> claims = new List<Claim>
                                {
                                    new Claim(ClaimTypes.Name, context.UserName, context.Options.ClaimsIssuer)
                                };

                                ClaimsPrincipal principal = new ClaimsPrincipal(
                                    new ClaimsIdentity(
                                        claims,
                                        BasicAuthenticationDefaults.AuthenticationScheme));
                                context.Principal = principal;

                                return Task.CompletedTask;
                            }
                        };
                    });
        }

        private void ConfigureJwtBearerAuthentication(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(
                    options =>
                    {
                        options.Authority = GetJwtBearerAuthenticationValue("Authority");
                        options.Audience = GetJwtBearerAuthenticationValue("Audience");
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            NameClaimType = GetJwtBearerAuthenticationValue("NameClaimType")
                        };
                    });
            //            services
            //                .AddAuthorization(
            //                    options =>
            //                    {
            //                        options.AddPolicy(
            //                            "SomePolicy", new AuthorizationPolicyBuilder()
            //                                .RequireAuthenticatedUser()
            //                                .AddAuthenticationSchemes("schemeName")
            //                                .RequireClaim("some-claim-slash-group")
            //                                .RequireRole("some-role-claim")
            //                                .Build());
            //                    });
        }

        private string GetJwtBearerAuthenticationValue(string value)
        {
            return Configuration.GetSection($"JwtBearerAuthentication:{value}").Value;
        }

        private void ConfigureApplicationServices(IServiceCollection services)
        {
            services.AddScoped<IValueService, ValueService>();
            services.AddScoped<IValueRepository, ValueRepository>();
        }

        /// <summary>
        ///     Map a configuration section to a type.
        /// </summary>
        private T GetConfigurationSection<T>(string sectionName)
        {
            IConfigurationSection section = Configuration.GetSection(sectionName);
            if (section == null)
            {
                throw new ApplicationException($"Missing {sectionName} section(ex. in AppSettings)");
            }

            T configuration = section.Get<T>();
            if (configuration == null)
            {
                throw new ApplicationException(
                    $"Unable to bind {sectionName} configuration section to type {typeof(T).FullName}.");
            }

            return configuration;
        }
    }
}
