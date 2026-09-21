namespace MyLoto.Application.Commands.Draws;

public record DrawLiveStatusResponse(
    string Status, 
    List<int> DrawnNumbers
);