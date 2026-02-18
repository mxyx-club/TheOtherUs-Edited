using TheOtherRoles.Objects;
using static TheOtherRoles.Buttons.HudManagerStartPatch;
using static TheOtherRoles.RPCProcedure;

namespace TheOtherRoles.Roles.Modifier;

public class Aftermath
{
    public static PlayerControl aftermath;
    public static Color color = new Color32(165, 255, 165, byte.MaxValue);

    public static void clearAndReload()
    {
        aftermath = null;
    }

    public static void afterTrigger(byte playerId, byte killerId)
    {
        var player = PlayerById(playerId);
        var killer = PlayerById(killerId);
        if (killer == null || killer == player) return;

        if (Blackmailer.Player == killer)
        {
            var target = killer;
            if (Blackmailer.currentTarget != null) target = Blackmailer.currentTarget;
            var writer = StartRPC(CustomRPC.BlackmailPlayer);
            writer.Write(target.PlayerId);
            writer.EndRPC();
            blackmailPlayer(target.PlayerId);
            blackmailerButton.Timer = blackmailerButton.MaxTimer;
        }
        else if (Bomber.bomber == killer)
        {
            var target = killer;
            if (Bomber.currentTarget != null) target = Bomber.currentTarget;
            var bombWriter = StartRPC(killer.NetId, CustomRPC.GiveBomb);
            bombWriter.Write(target.PlayerId);
            bombWriter.Write(false);
            bombWriter.EndRPC();
            giveBomb(target.PlayerId);
            bomberBombButton.Timer = bomberBombButton.MaxTimer;
        }
        else if (Terrorist.terrorist == killer)
        {
            var pos = PlayerControl.LocalPlayer.transform.position;

            terroristButton.HasEffect = !Terrorist.selfExplosion;

            var writer = StartRPC(CustomRPC.PlaceBomb);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            writer.Write(pos);
            writer.EndRPC();
            placeBomb(PlayerControl.LocalPlayer, pos);

            terroristButton.Timer = terroristButton.MaxTimer;
        }
        else if (Morphling.morphling == killer)
        {
            var writer = StartRPC(CustomRPC.MorphlingMorph);
            writer.Write(player.PlayerId);
            writer.EndRPC();
            morphlingMorph(player.PlayerId);
            Morphling.sampledTarget = null;
            morphlingButton.Timer = Morphling.duration;
            SoundEffectsManager.play("morphlingMorph");
        }
        else if (Butcher.butcher == killer)
        {
            if (Butcher.dissectedId == byte.MaxValue) return;
            var list = new List<Vector3>();
            list.AddRange(MapData.MapSpawnPosition(false));
            list.AddRange(MapData.FindVentSpawnPositions(false));
            list.Shuffle();

            for (var i = 0; i < Butcher.dissectedBodyCount; i++)
            {
                var pos = list.RandomTake();
                var writer = StartRPC(CustomRPC.CreateDeadBody);
                writer.Write(Butcher.dissectedId);
                writer.Write(pos);
                writer.Write(i);
                writer.EndRPC();
                CreateDeadBody(Butcher.dissectedId, pos, i);
                list.Remove(pos);
            }

            Butcher.dissectedId = byte.MaxValue;
            Butcher.canDissection = false;
            SoundEffectsManager.play("cleanerClean");
        }
        else if (Witch.witch == killer)
        {
            var target = killer;
            if (Witch.currentTarget != null) target = Witch.currentTarget;
            var writer = StartRPC(CustomRPC.SetFutureSpelled);
            writer.Write(target.PlayerId);
            writer.EndRPC();
            setFutureSpelled(target.PlayerId);
            SoundEffectsManager.play("witchSpell");
            witchSpellButton.Timer = witchSpellButton.MaxTimer;
        }/*
        else if (Warlock.warlock == killer)
        {

        }*/
        else if (Miner.miner == killer)
        {
            var writer = StartRPC(CustomRPC.Mine);
            var pos = killer.transform.position;
            var buff = new byte[sizeof(float) * 2];
            Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
            Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));
            var id = getAvailableId();
            writer.Write(id);
            writer.Write(killer.PlayerId);
            writer.WriteBytesAndSize(buff);
            writer.Write(0.01f);
            writer.EndRPC();
            Mine(id, buff, 0.01f);
            minerMineButton.Timer = minerMineButton.MaxTimer;
        }
        else if (Yoyo.yoyo == killer)
        {
            var pos = PlayerControl.LocalPlayer.transform.position;

            if (Yoyo.markedLocation == null)
            {
                Message($"marked location is null in button press");
                var writer = StartRPC(CustomRPC.YoyoMarkLocation);
                writer.Write(pos);
                writer.EndRPC();
                yoyoMarkLocation(pos);
                SoundEffectsManager.play("tricksterPlaceBox");
                yoyoButton.Sprite = Yoyo.blinkButtonSprite;
                yoyoButton.Timer = 10f;
                yoyoButton.HasEffect = false;
                yoyoButton.buttonText = "BlinkText".Translate();
            }
            else
            {
                Message("in else for some reason");
                // Jump to location
                Message($"trying to blink!");
                var exit = (Vector3)Yoyo.markedLocation;
                if (SubmergedCompatibility.IsSubmerged)
                {
                    SubmergedCompatibility.ChangeFloor(exit.y > -7);
                }
                var writer = StartRPC(killer.NetId, CustomRPC.YoyoBlink);
                writer.Write(true);
                writer.Write(pos);
                writer.EndRPC();
                yoyoBlink(true, pos);
                yoyoButton.EffectDuration = Yoyo.blinkDuration;
                yoyoButton.Timer = 10f;
                yoyoButton.HasEffect = true;
                yoyoButton.buttonText = "ReturningText".Translate();
                SoundEffectsManager.play("morphlingMorph");
            }
        }
        else if (EvilTrapper.evilTrapper == killer)
        {
            EvilTrapper.setTrap();
            evilTrapperSetTrapButton.Timer = evilTrapperSetTrapButton.MaxTimer;
        }
        else if (Trickster.trickster == killer)
        {
            if (!JackInTheBox.hasJackInTheBoxLimitReached())
            {
                var pos = Trickster.trickster.transform.position;

                var writer = StartRPC(killer.NetId, CustomRPC.PlaceJackInTheBox);
                writer.Write(Trickster.trickster.PlayerId);
                writer.Write(pos);
                writer.EndRPC();
                placeJackInTheBox(Trickster.trickster, pos);
                SoundEffectsManager.play("tricksterPlaceBox");
                placeJackInTheBoxButton.Timer = placeJackInTheBoxButton.MaxTimer;
            }
            else
            {
                var writer = StartRPC(CustomRPC.LightsOut);
                writer.EndRPC();
                lightsOut();
                SoundEffectsManager.play("lighterLight");
                lightsOutButton.Timer = lightsOutButton.MaxTimer;
            }
        }
        else if (Undertaker.undertaker == killer)
        {
            if (Undertaker.dragedBody != null)
            {
                var writer = StartRPC(CustomRPC.jesterDragBody);
                writer.Write(byte.MaxValue);
                writer.EndRPC();
                Undertaker.DragBody(byte.MaxValue);
            }
            else if (Undertaker.targetBody != null)
            {
                var writer = StartRPC(CustomRPC.jesterDragBody);
                writer.Write(Undertaker.targetBody.ParentId);
                writer.EndRPC();
                Undertaker.DragBody(Undertaker.targetBody.ParentId);
            }
        }
        else if (Cleaner.cleaner == killer)
        {
            var db = GetDeadBody(PlayerControl.LocalPlayer.GetTruePosition());

            var writer = StartRPC(CustomRPC.CleanBody);
            writer.Write(db.ParentId);
            writer.Write(Cleaner.cleaner.PlayerId);
            writer.EndRPC();
            cleanBody(db.ParentId, Cleaner.cleaner.PlayerId);
            SoundEffectsManager.play("cleanerClean");
            cleanerCleanButton.Timer = cleanerCleanButton.MaxTimer;
        }
        else if (Eraser.eraser == killer)
        {
            var target = killer;
            if (Eraser.currentTarget != null) target = Eraser.currentTarget;
            var writer = StartRPC(CustomRPC.SetFutureErased);
            writer.Write(target.PlayerId);
            writer.EndRPC();
            setFutureErased(target.PlayerId);
            SoundEffectsManager.play("eraserErase");
            eraserButton.Timer = eraserButton.MaxTimer;
        }
        else if (Camouflager.camouflager == killer)
        {
            var writer = StartRPC(CustomRPC.CamouflagerCamouflage);
            writer.Write(1);
            writer.EndRPC();
            camouflagerCamouflage(1);
            SoundEffectsManager.play("morphlingMorph");
            camouflagerButton.Timer = camouflagerButton.MaxTimer;
        }
        else if (Grenadier.Player == killer)
        {
            var writer = StartRPC(CustomRPC.GrenadierFlash);
            writer.Write(false);
            writer.EndRPC();
            grenadierFlash(false);
            grenadierFlashButton.Timer = grenadierFlashButton.MaxTimer + Grenadier.duration;
        }
        else if (Swooper.swooper == killer)
        {
            var writer = StartRPC(CustomRPC.SetSwoop);
            writer.Write(killer.PlayerId);
            writer.Write(byte.MinValue);
            writer.EndRPC();
            setSwoop(Swooper.swooper.PlayerId, byte.MinValue);
            swooperSwoopButton.Timer = swooperSwoopButton.MaxTimer + Swooper.duration;
        }
        else if (Jackal.jackal.Any(x => x == killer) && Jackal.canSwoop)
        {
            var writer = StartRPC(CustomRPC.SetJackalSwoop);
            writer.Write(killer.PlayerId);
            writer.Write(byte.MinValue);
            writer.EndRPC();
            setJackalSwoop(killer.PlayerId, byte.MinValue);
            jackalSwoopButton.Timer = jackalSwoopButton.MaxTimer + Jackal.duration;
        }
        else if (Marionette.Player == killer)
        {
            if (Marionette.decoy == null)
            {
                var writer = StartRPC(CustomRPC.PlaceDecoy);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(PlayerControl.LocalPlayer.transform.position);
                writer.EndRPC();
                PlaceDecoy(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer.transform.position);
                marionettePlaceButton.Timer = marionettePlaceButton.MaxTimer;

                Marionette.marionetteMode = 0;

                Marionette.SetMarionetteMode(0);
                marionetteButton.Timer = marionetteButton.MaxTimer;
            }
            else
            {

                var writer = StartRPC(CustomRPC.DecoySwap);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(Marionette.decoy.Id);
                writer.Write(PlayerControl.LocalPlayer.transform.position);
                writer.Write(Marionette.decoy.GameObject.transform.position);
                writer.EndRPC();
                DecoySwap(PlayerControl.LocalPlayer, Marionette.decoy.Id, PlayerControl.LocalPlayer.transform.position, Marionette.decoy.GameObject.transform.position);

                if (HudManager.Instance.PlayerCam.Target != PlayerControl.LocalPlayer) HudManager.Instance.PlayerCam.SetTargetWithLight(PlayerControl.LocalPlayer);
            }
        }
    }
}
