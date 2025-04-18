using System;
using GameStore.Api.Dtos;

namespace GameStore.Api.Endpoints;

public static class GameEndponts
{

private static readonly List<GameDto> games = [
    new (1,
    "Street Fighter ",
    "RolePlaying",
    59.99M,
    new DateOnly(2010,9,30)),
    new (2,
    "Final Fantasy XIV",
    "RolePlaying",
    59.99M,
    new DateOnly(2010,9,30)),
    new (3,
    "FIFA 23",
    "Sports",
    69.99M,
    new DateOnly(2022,9,27))
];
    public static RouteGroupBuilder MapGamesEndPoints(this WebApplication app){
        const string GetEndPointName = "GetGame";
        var group = app.MapGroup("games").WithParameterValidation();;
        // Get games
        group.MapGet("/", () => games);

        // Get games/1
        group.MapGet("/{id}", (int id) => {

            GameDto? game = games.Find(g => g.Id == id);
            return game is null ? Results.NotFound() : Results.Ok(game);
        }).WithName(GetEndPointName);

        // Post games
        group.MapPost("/",(CreateGameDto NewGame) => 
        {
            if(string.IsNullOrEmpty(NewGame.Name)){
                return Results.BadRequest("Name is required");
            };

            GameDto game = new (
                games.Count + 1,
                NewGame.Name,
                NewGame.Genre,
                NewGame.Price,
                NewGame.ReleaseDate);
            games.Add(game);

            return Results.CreatedAtRoute(GetEndPointName, new {id = game.Id},game);
        });


        // PUT /games
        group.MapPut("/{id}",(int id, UpdateGameDto updateGameDto) => {

            var index = games.FindIndex(game => game.Id == id);
            if(index == -1 ){
                return Results.NotFound();
            }else{
                games[index] = new GameDto (
                    id,
                    updateGameDto.Name,
                    updateGameDto.Genre,
                    updateGameDto.Price,
                    updateGameDto.ReleaseDate
                ); 
                return Results.NoContent();
            };
        });

        // Delete /games
        group.MapDelete("/{id}",(int id) => {
            games.RemoveAll(game => game.Id == id);
            return Results.NoContent();
        });
        return group;
    }
};