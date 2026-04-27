using CommunityToolkit.Mvvm.Messaging.Messages;
using PandaCubeTimer.Models;

namespace PandaCubeTimer.Messages;

public class ActiveSessionChangedMessage(Session value) : ValueChangedMessage<Session>(value);