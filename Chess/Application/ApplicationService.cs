using Chess.Core.Match;
using Chess.Core.Match.Commands;

namespace Chess.Application;

public class ApplicationService
{
    //Return result
    public void StartMatch(StartMatch command)
    {

        //Build aggregate
        var match = MatchFactory.Create();

        match.Start(command);

        //publish event

        // save aggegrate


    }

}

