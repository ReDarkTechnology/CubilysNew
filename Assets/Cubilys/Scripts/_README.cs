namespace Cubilys
{
    public class _README
    {
        public const string intro = "I'm not sure why you would inspect the code of this game... " +
            "There's nothing much important here unless you just wanted to cheat... " +
            "Although, it's fine, the mechanics are just made to prevent people from wasting their time too much. " +
            "Preferably, don't distribute the modified version of the game...";
        public const string content = "I made this game alone, the Unity version used isn't as secure as new ones. " +
            "So you probably can decompile the assemblies, and the IL2CPP took so long to build so I just- " +
            "Decided to use Mono as the scripting backend, besides, now you can read this README script file!" +
            "I also don't have any money to publish the game to Play Store or App Store, sorry for that";
        public const string outro = "Afterall, thank you for reading this. You can probably donate me if you want- " +
            "I haven't made any income from my fan-levels so it would be greatly appreciated if you do! " +
            "Here's the link - https://ko-fi.com/bunzhizendi";


        public const string moddingGuide = "I'd even kindly give you the instructions on how to modify this game" +
            "There's a class called LevelManager, it has some classic MonoBehaviour structure" +
            "You can put 'RenderSettings.fog = false' in the 'void Start()' if you want the game without the fog" +
            " - " +
            "You can also put 'var t = GameObject.FindGameObjectsWithTag(\"Obstacle\"); foreach (var o in t) { o.tag = \"Untagged\"; }'" +
            "with the reversed slash removed to make the line immortal!" +
            " - " +
            "On some others, you had to figure it out yourself, there's lots of stuff you can do with the class. Good luck! :D";
    }
}
