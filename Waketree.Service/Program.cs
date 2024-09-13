using Waketree.Service;
using Waketree.Service.Singletons;

// set service as "up"
var state = State.GetState();
state.ServiceState = Waketree.Common.ServiceStates.Up;

// build the app
HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<WebService>();
builder.Services.AddHostedService<UDPMessageListenerService>();
builder.Services.AddHostedService<UDPMessageProcessorService>();
builder.Services.AddHostedService<SupervisorConnectionService>();
builder.Services.AddHostedService<ServiceOperationProcessorService>();
var app = builder.Build();
state.App = app;

// go!
state.App.RunAsync();

// wait
Console.ReadLine();

// notify supervisor and end
if (state.ServiceState == Waketree.Common.ServiceStates.Connected)
{
    UDPMessages.ServiceDisconnecting(state.SupervisorIP);
}
Environment.Exit(0);