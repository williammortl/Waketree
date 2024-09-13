using Waketree.Supervisor;
using Waketree.Supervisor.Singletons;

System.Net.ServicePointManager.ServerCertificateValidationCallback +=
    (s, cert, chain, sslPolicyErrors) => true;

// set service as "up"
var state = State.GetState();

// build the app
HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<WebService>();
builder.Services.AddHostedService<UDPMessageListenerService>();
builder.Services.AddHostedService<UDPMessageProcessorService>();
builder.Services.AddHostedService<ServiceStatusUpdaterService>();
builder.Services.AddHostedService<SupervisorOperationProcessorService>();
var app = builder.Build();
state.App = app;

// go!
state.App.StartAsync();

// wait
Console.ReadLine();

// notify services and end
UDPMessages.BroadcastShutdownToServices();
Environment.Exit(0);