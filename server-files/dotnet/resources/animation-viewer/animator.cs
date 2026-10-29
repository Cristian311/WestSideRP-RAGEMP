using System;
using System.Collections.Generic;
using System.IO;
using GTANetworkAPI;

public class Animator : Script
{

    [Flags]
    public enum AnimationFlags
    {
        Loop = 1 << 0,
        StopOnLastFrame = 1 << 1,
        OnlyAnimateUpperBody = 1 << 4,
        AllowPlayerControl = 1 << 5,
        Cancellable = 1 << 7
    }

    [RemoteEvent("ANIMATOR_EVENT")]
    public void AnimatorEventHandler(Player player, params object[] args)
    {
        switch((string)args[0])
        {
            case "Animator_PlayAnim": PlayAnimation(player, (string)args[1]); break;
        }
    }

    private void PlayAnimation(Player player, string anim)
    {
        string[] animInfo = anim.ToString().Split(' ');
        player.SetData("PLAYED_ANIMATION_GROUP", animInfo[0]);
        player.SetData("PLAYED_ANIMATION_NAME", animInfo[1]);
        player.StopAnimation();
        player.PlayAnimation(animInfo[0], animInfo[1], (int)AnimationFlags.Loop);
    }

    [Command("animator", "====================[~b~ANIM~w~ATOR]====================\n"+
        "Launch: ~g~/animator start \n" +
        "~w~Help: ~g~/animator help \n" +
        "~w~Stop animation: ~g~/animator stop \n" +
        "~w~Save animation: ~g~/animator save [savename] \n" +
        "~w~Skip to animation: ~g~/animator skip [animation id]")]
    public void StartAnimator(Player player, string action = null, string action2 = "Anim")
    {
        if (!player.HasData("ANIMATOR_OPEN")) player.SetData("ANIMATOR_OPEN", false);
        bool AnimatorOpen = player.GetData<bool>("ANIMATOR_OPEN");

        if (action == null)
        {
            if (!AnimatorOpen)
            {
                player.SetData("ANIMATOR_OPEN", true);
                player.TriggerEvent("StartClientAnimator");

                int animIndex = 0;
                List<string> temp = new List<string>();
                foreach(string anim in animator.animations.AllAnimations)
                {
                    temp.Add(anim);
                    animIndex++;

                    if (animIndex == 500)
                    {
                        player.TriggerEvent("AddAnimatorAnims", NAPI.Util.ToJson(temp));
                        animIndex = 0;
                        temp.Clear();
                    }

                }
                player.SendChatMessage("~b~[ANIMATOR]: ~w~Animator is now ~g~ON~w~. Type ~g~/animator help ~w~for more options.");
            }
            else
            {
                player.ResetData("ANIMATOR_OPEN");
                player.TriggerEvent("StopClientAnimator");
                player.StopAnimation();
                player.SendChatMessage("~b~[ANIMATOR]: ~w~Animator is now ~r~OFF~w~.");
            }
        }

        if(action != null && AnimatorOpen)
        {
            if (AnimatorOpen)
            {
                if (action == "save") SaveAnimatorData(player, action2);
                if (action == "skip") SkipAnimatorData(player, action2);
                if (action == "help")
                {
                    player.SendChatMessage("=================================[~b~ANIM~w~ATOR]================================");
                    player.SendChatMessage("Use ~y~LEFT ~w~and ~y~RIGHT ~w~arrow keys to cycle through animations.");
                    player.SendChatMessage("Use ~y~UP ~w~and ~y~DOWN ~w~arrow keys to cycle animations by one hundred instances.");
                    player.SendChatMessage("You may also skip to a specific animation ID, just use ~y~/animator skip [number]~w~.");
                    player.SendChatMessage("If you wish to save your animations into .txt file, use ~y~/animator save [savename]~w~.");
                    player.SendChatMessage("~w~Please remember that some animations are not meant to be used by peds, or");
                    player.SendChatMessage("are meant to be used in specific circumstances, therefore ~r~may not work~w~!");
                }
                if(action == "stop")
                {
                    player.SendChatMessage("~b~[ANIMATOR]: ~w~Animation have been stopped.");
                    player.StopAnimation();
                }
            } else
            {
                player.SendChatMessage("~b~[ANIMATOR]: ~r~You have to launch the animator first! ~y~/animator.");
            }
        }
    }

    public void SaveAnimatorData(Player player, string name)
    {
        string anim_group = player.GetData<string>("PLAYED_ANIMATION_GROUP");
        string anim_name = player.GetData<string>("PLAYED_ANIMATION_NAME");
        File.AppendAllText("Saved_Animations.txt", string.Format("{0}:          {1} {2}", name, anim_group, anim_name) + Environment.NewLine);
        player.SendChatMessage(string.Format("~b~[ANIMATOR]: ~w~Animation saved! Name: ~g~{0} ~w~Anim: ~y~{1} ~b~{2}", name, anim_group, anim_name));
        player.SendNotification("Saved animation as ~b~" + name + "!");
    }

    public void SkipAnimatorData(Player player, string animationID)
    {
        int ID;
        if (Int32.TryParse(animationID, out ID))
        {
            int animations_amount = animator.animations.AllAnimations.Count - 1;
            if (ID > animations_amount || ID < 0)
            {
                player.SendChatMessage("~b~[ANIMATOR]: ~w~ID has to be between 0 and "+ animations_amount+"!");
                return;
            }
            player.TriggerEvent("SkipAnimatorData", ID);
        } else {
            player.SendChatMessage("~b~[ANIMATOR]: ~r~Wrong number format!");
        }
    }
}