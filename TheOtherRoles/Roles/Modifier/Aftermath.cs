using System;
using Hazel;
using TheOtherRoles.Objects;
using UnityEngine;
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
        var player = playerById(playerId);
        var killer = playerById(killerId);
        if (killer == null || killer == player) return;

        if (Blackmailer.blackmailer == killer)
        {
            var target = killer;
            if (Blackmailer.currentTarget != null) target = Blackmailer.currentTarget;
            var writer = AmongUsClient.Instance.StartRpcImmediately(killer.NetId,
                (byte)CustomRPC.BlackmailPlayer, SendOption.Reliable);
            writer.Write(target.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
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
            if (checkMuderAttempt(Terrorist.terrorist, Terrorist.terrorist) != MurderAttemptResult.BlankKill)
            {
                var pos = killer.transform.position;
                var buff = new byte[sizeof(float) * 2];
                Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));
                var writer = AmongUsClient.Instance.StartRpc(killer.NetId, (byte)CustomRPC.PlaceBomb);
                writer.WriteBytesAndSize(buff);
                writer.EndMessage();
                placeBomb(buff);
                SoundEffectsManager.play(Terrorist.selfExplosion ? "bombExplosion" : "trapperTrap");

                if (Terrorist.selfExplosion)
                {
                    var loacl = Terrorist.terrorist.PlayerId;
                    var writer1 = AmongUsClient.Instance.StartRpcImmediately(Terrorist.terrorist.NetId,
                        (byte)CustomRPC.UncheckedMurderPlayer, SendOption.Reliable);
                    writer1.Write(loacl);
                    writer1.Write(loacl);
                    writer1.Write(byte.MaxValue);
                    AmongUsClient.Instance.FinishRpcImmediately(writer1);
                    uncheckedMurderPlayer(loacl, loacl, byte.MaxValue);
                }
            }
            terroristButton.Timer = terroristButton.MaxTimer;
        }
        else if (Morphling.morphling == killer)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(killer.NetId,
                (byte)CustomRPC.MorphlingMorph, SendOption.Reliable);
            writer.Write(player.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            morphlingMorph(player.PlayerId);
            Morphling.sampledTarget = null;
            morphlingButton.Timer = Morphling.duration;
            SoundEffectsManager.play("morphlingMorph");
        }
        else if (Butcher.butcher == killer)
        {

            foreach (var collider2D in Physics2D.OverlapCircleAll(
                         PlayerControl.LocalPlayer.GetTruePosition(),
                         PlayerControl.LocalPlayer.MaxReportDistance, Constants.PlayersOnlyMask))
            {
                if (collider2D.tag == "DeadBody")
                {
                    var component = collider2D.GetComponent<DeadBody>();
                    if (component && !component.Reported)
                    {
                        var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
                        var truePosition2 = component.TruePosition;
                        if (Vector2.Distance(truePosition2, truePosition) <=
                            PlayerControl.LocalPlayer.MaxReportDistance &&
                            PlayerControl.LocalPlayer.CanMove &&
                            !PhysicsHelpers.AnythingBetween(truePosition, truePosition2,
                                Constants.ShipAndObjectsMask, false))
                        {
                            var playerInfo = GameData.Instance.GetPlayerById(component.ParentId);

                            var writer = AmongUsClient.Instance.StartRpcImmediately(Butcher.butcher.NetId,
                                (byte)CustomRPC.DissectionBody, SendOption.Reliable);
                            writer.Write(playerInfo.PlayerId);
                            writer.Write(Butcher.butcher.PlayerId);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            dissectionBody(playerInfo.PlayerId, Butcher.butcher.PlayerId);

                            Butcher.canDissection = false;
                            SoundEffectsManager.play("cleanerClean");
                            break;
                        }
                    }
                }
            }
        }
        else if (Witch.witch == killer)
        {
            var target = killer;
            if (Witch.currentTarget != null) target = Witch.currentTarget;
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.SetFutureSpelled, SendOption.Reliable);
            writer.Write(target.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            setFutureSpelled(target.PlayerId);
            SoundEffectsManager.play("witchSpell");
            witchSpellButton.Timer = witchSpellButton.MaxTimer;
        }/*
        else if (Warlock.warlock == killer)
        {

        }*/
        else if (Miner.miner == killer)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(killer.NetId,
                (byte)CustomRPC.Mine, SendOption.Reliable);
            var pos = killer.transform.position;
            var buff = new byte[sizeof(float) * 2];
            Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
            Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));
            var id = getAvailableId();
            writer.Write(id);
            writer.Write(killer.PlayerId);
            writer.WriteBytesAndSize(buff);
            writer.Write(0.01f);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            Mine(id, buff, 0.01f);
            minerMineButton.Timer = minerMineButton.MaxTimer;
        }
        else if (Escapist.escapist == killer)
        {
            if (Escapist.escapeLocation != Vector3.zero)
            {
                killer.NetTransform.RpcSnapTo(Escapist.escapeLocation);
            }
            else
            {
                Escapist.escapeLocation = PlayerControl.LocalPlayer.transform.localPosition;
            }
            escapistMarkButton.Timer = escapistMarkButton.MaxTimer;
            escapistEscapeButton.Timer = escapistEscapeButton.MaxTimer;
        }
        else if (Yoyo.yoyo == killer)
        {
            var pos = PlayerControl.LocalPlayer.transform.position;
            byte[] buff = new byte[sizeof(float) * 2];
            Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
            Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

            if (Yoyo.markedLocation == null)
            {
                Message($"marked location is null in button press");
                var writer = AmongUsClient.Instance.StartRpc(killer.NetId, (byte)CustomRPC.YoyoMarkLocation, SendOption.Reliable);
                writer.WriteBytesAndSize(buff);
                writer.EndMessage();
                yoyoMarkLocation(buff);
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
                var writer = AmongUsClient.Instance.StartRpc(killer.NetId, (byte)CustomRPC.YoyoBlink, SendOption.Reliable);
                writer.Write(byte.MaxValue);
                writer.WriteBytesAndSize(buff);
                writer.EndMessage();
                yoyoBlink(true, buff);
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
                var pos = PlayerControl.LocalPlayer.transform.position;
                var buff = new byte[sizeof(float) * 2];
                Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                var writer = AmongUsClient.Instance.StartRpc(killer.NetId,
                    (byte)CustomRPC.PlaceJackInTheBox);
                writer.WriteBytesAndSize(buff);
                writer.EndMessage();
                placeJackInTheBox(buff);
                SoundEffectsManager.play("tricksterPlaceBox");
                placeJackInTheBoxButton.Timer = placeJackInTheBoxButton.MaxTimer;
            }
            else
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(killer.NetId,
                    (byte)CustomRPC.LightsOut, SendOption.Reliable);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
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
            foreach (var collider2D in Physics2D.OverlapCircleAll(
                PlayerControl.LocalPlayer.GetTruePosition(),
                PlayerControl.LocalPlayer.MaxReportDistance, Constants.PlayersOnlyMask))
            {
                if (collider2D.tag == "DeadBody")
                {
                    var component = collider2D.GetComponent<DeadBody>();
                    if (component && !component.Reported)
                    {
                        var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
                        var truePosition2 = component.TruePosition;
                        if (Vector2.Distance(truePosition2, truePosition) <=
                            PlayerControl.LocalPlayer.MaxReportDistance &&
                            PlayerControl.LocalPlayer.CanMove &&
                            !PhysicsHelpers.AnythingBetween(truePosition, truePosition2,
                                Constants.ShipAndObjectsMask, false))
                        {
                            var playerInfo = GameData.Instance.GetPlayerById(component.ParentId);

                            var writer = AmongUsClient.Instance.StartRpcImmediately(killer.NetId,
                                (byte)CustomRPC.CleanBody, SendOption.Reliable);
                            writer.Write(playerInfo.PlayerId);
                            writer.Write(Cleaner.cleaner.PlayerId);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            cleanBody(playerInfo.PlayerId, Cleaner.cleaner.PlayerId);

                            Cleaner.cleaner.killTimer = cleanerCleanButton.Timer = cleanerCleanButton.MaxTimer;
                            SoundEffectsManager.play("cleanerClean");
                            break;
                        }
                    }
                }
            }

            cleanerCleanButton.Timer = cleanerCleanButton.MaxTimer;
        }
        else if (Eraser.eraser == killer)
        {
            var target = killer;
            if (Eraser.currentTarget != null) target = Eraser.currentTarget;
            var writer = AmongUsClient.Instance.StartRpcImmediately(killer.NetId,
                (byte)CustomRPC.SetFutureErased, SendOption.Reliable);
            writer.Write(target.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            setFutureErased(target.PlayerId);
            SoundEffectsManager.play("eraserErase");
            eraserButton.Timer = eraserButton.MaxTimer;
        }
        else if (Camouflager.camouflager == killer)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(killer.NetId,
                (byte)CustomRPC.CamouflagerCamouflage, SendOption.Reliable);
            writer.Write(1);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            camouflagerCamouflage(1);
            SoundEffectsManager.play("morphlingMorph");
            camouflagerButton.Timer = camouflagerButton.MaxTimer;
        }
        else if (Grenadier.Player == killer)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(killer.NetId,
                (byte)CustomRPC.GrenadierFlash, SendOption.Reliable);
            writer.Write(false);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            grenadierFlash(false);
            grenadierFlashButton.Timer = grenadierFlashButton.MaxTimer + Grenadier.duration;
        }
        else if (Swooper.swooper == killer)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(killer.NetId,
                (byte)CustomRPC.SetSwoop, SendOption.Reliable);
            writer.Write(killer.PlayerId);
            writer.Write(byte.MinValue);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            setSwoop(Swooper.swooper.PlayerId, byte.MinValue);
            swooperSwoopButton.Timer = swooperSwoopButton.MaxTimer + Swooper.duration;
        }
        else if (Jackal.jackal.Any(x => x == killer) && Jackal.canSwoop)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(killer.NetId,
                (byte)CustomRPC.SetJackalSwoop, SendOption.Reliable);
            writer.Write(killer.PlayerId);
            writer.Write(byte.MinValue);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            setJackalSwoop(killer.PlayerId, byte.MinValue);
            jackalSwoopButton.Timer = jackalSwoopButton.MaxTimer + Jackal.duration;
        }
    }
}
