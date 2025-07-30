using CalloutInterfaceAPI;
using DialogueSystem.API;
using DialogueSystem.UI;
using EverydayCallouts.Engine;
using EverydayCallouts.Engine.Mood;
using EverydayCallouts.Logging;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.PauseMenu;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EverydayCallouts.Callouts
{
    [CalloutInterface("Noise Complaint", CalloutProbability.High, "Caller is stating their neibhor is playing music to loud", "Code 2", "LSPD")]
    // [CalloutInterface("Your callout name", CalloutProbability.Medium, "A very useful description", "Code 2", "LSPD")]

    public class NoiseComplaint : Callout
    {

        private enum NoiseComplaintStage
        {
            None,
            ApproachCaller,
            TalkToCaller,
            CallerWalksToDoor,
            WalkToMarker,
            SpawnNeighbor,
            TalkToNeighbor,
            Done
        }

        private NoiseComplaintStage currentStage = NoiseComplaintStage.None;
        private GameFiber calloutFiber;
        private bool calloutActive = false;

        // Characters
        private Ped Caller;
        private Ped Neighbor;

        // Blips
        private Blip CallerBlip;
        private Blip NeighborBlip;
        private Blip calloutBlip;

        // Positions
        private Vector3 SpawnPoint;
        private Vector4 CallerPos;
        private Vector4 NeighborPos;
        private Vector3 MarkerPos;

        // Dialog / State Tracking
        private Conversation callerconvo;
        private Conversation neighborconvo;
        private UIMenu callerMenu = new UIMenu("Caller", "Talk to the caller");
        private UIMenu neighborMenu = new UIMenu("Noisy Neighbor", "Talk to the neighbor");
        private bool hasTalkedToCaller = false;
        private bool hasdisplayedintrosubtitle = false;
        private bool markerCreated = false;

        private List<(Vector4 SpawnPoint, Vector4 CallerSpawn, Vector4 NeighborSpawn, Vector3 MarkerSpawn)> _spawnGroups = new List<(Vector4, Vector4, Vector4, Vector3)>
        {
            (
                new Vector4(-1054.59f, -1003.173f, 1.150192f, 110.9069f),
                new Vector4(-1056.478f, -1000.202f, 1.150192f, 126.4022f),
                new Vector4(-1054.645f, -999.9692f, 5.41049f, 189.0671f),
                new Vector3(-1055.6067f, -1001.0279f, 5.4005f)
            )
        };

        public override bool OnBeforeCalloutDisplayed()
        {
            Vector3 playerPos = Game.LocalPlayer.Character.Position;

            (float Distance, (Vector4 SpawnPoint, Vector4 CallerSpawn, Vector4 NeighborSpawn, Vector3 MarkerSpawn) Group) closest =
            (float.MaxValue, default((Vector4, Vector4, Vector4, Vector3)));


            foreach (var group in _spawnGroups)
            {
                Vector3 spawn = new Vector3(group.SpawnPoint.X, group.SpawnPoint.Y, group.SpawnPoint.Z);
                float distance = playerPos.DistanceTo(spawn);

                if (distance < closest.Distance)
                {
                    closest = (distance, group);
                }
            }

            var closestGroup = closest.Group;

            // Use values from the closest group
            SpawnPoint = new Vector3(closestGroup.SpawnPoint.X, closestGroup.SpawnPoint.Y, closestGroup.SpawnPoint.Z);
            CallerPos = closestGroup.CallerSpawn;
            NeighborPos = closestGroup.NeighborSpawn;
            MarkerPos = closestGroup.MarkerSpawn;


            if (closest.Distance > 550f)
            {
                return false;
            }

            SpawnPoint = new Vector3(closestGroup.SpawnPoint.X, closestGroup.SpawnPoint.Y, closestGroup.SpawnPoint.Z);
            CallerPos = closestGroup.CallerSpawn;
            NeighborPos = closestGroup.NeighborSpawn;

            AddMinimumDistanceCheck(100f, SpawnPoint);
            AddMaximumDistanceCheck(550f, SpawnPoint);
            ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 20f);

            LSPD_First_Response.Mod.API.Functions.PlayScannerAudioUsingPosition("CITIZENS_REPORT CRIME_DISTURBING_THE_PEACE IN_OR_ON_POSITION UNITS_RESPOND_CODE_02", SpawnPoint);

            CalloutPosition = SpawnPoint;

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Caller = new Ped(new Vector3(CallerPos.X, CallerPos.Y, CallerPos.Z), CallerPos.W);

            if (!Caller.Exists())
            {
                return false;
            }

            Caller.IsPersistent = true;
            Caller.CanPlayGestureAnimations = false;
            Caller.CanPlayAmbientAnimations = false;
            PedMood mood = PedMoodManager.SetMood(Caller);
            DialogueConditions.currentPed = Caller;

            CalloutInfoMessages.PedCreated();

            callerconvo = Loader.LoadDialogue("EverydayCallouts/NoiseComplaint/CallerDialogue.json", callerMenu);
            neighborconvo = Loader.LoadDialogue("EverydayCallouts/NoiseComplaint/neighborDialogue.json", neighborMenu);

            callerconvo.Init();
            neighborconvo.Init();
            MenuManager.Pool.Add(callerMenu);
            MenuManager.Pool.Add(neighborMenu);
            MenuManager.StartProcessing();

            Caller.Tasks.PlayAnimation(
                "friends@frj@ig_1",
                "wave_a",
                1.0f,
                AnimationFlags.Loop | AnimationFlags.UpperBodyOnly
            );

            calloutBlip = new Blip(SpawnPoint, 15f);
            calloutBlip.Color = System.Drawing.Color.Yellow;
            calloutBlip.Alpha = 0.5f;
            calloutBlip.Name = "Callout Area";
            calloutBlip.IsRouteEnabled = true;

            calloutActive = true;
            currentStage = NoiseComplaintStage.ApproachCaller;
            calloutFiber = GameFiber.StartNew(RunCallout);

            return base.OnCalloutAccepted();
        }

        private void RunCallout()
        {
            try
            {
                while (calloutActive)
                {
                    switch (currentStage)
                    {
                        case NoiseComplaintStage.ApproachCaller:
                            if (!hasdisplayedintrosubtitle && Game.LocalPlayer.Character.DistanceTo(Caller.Position) < 25f && Game.LocalPlayer.Character.IsInAnyVehicle(true) && Caller.IsAlive)
                            {
                                Game.DisplaySubtitle("Officer, over here!");
                                hasdisplayedintrosubtitle = true;

                                CallerBlip = Caller.AttachBlip();
                                CallerBlip.Color = System.Drawing.Color.Yellow;
                                CallerBlip.Name = "Caller";

                                if (calloutBlip != null && calloutBlip.Exists())
                                    calloutBlip.Delete();

                                CalloutInterfaceAPI.Functions.SendMessage(this, "Officer is making contact with caller");
                            }

                            if (hasdisplayedintrosubtitle && Game.LocalPlayer.Character.Position.DistanceTo(Caller.Position) < 15f && !Game.LocalPlayer.Character.IsInAnyVehicle(false) && Caller.IsAlive)
                            {
                                CallerBlip.IsRouteEnabled = false;
                                Caller.Tasks.Clear();
                                Caller.Tasks.GoToOffsetFromEntity(Game.LocalPlayer.Character, -1, 3.5f, 0f, 0.9f);
                                Caller.KeepTasks = true;

                                currentStage = NoiseComplaintStage.TalkToCaller;
                            }
                            break;

                        case NoiseComplaintStage.TalkToCaller:
                            if (Game.LocalPlayer.Character.DistanceTo(Caller.Position) < 3f && !Game.LocalPlayer.Character.IsInAnyVehicle(false) && Caller.IsAlive)
                            {
                                Caller.Tasks.Clear();
                                Game.DisplayHelp("Press ~y~Y~s~ to talk to the caller", false);

                                if (Game.IsKeyDown(Keys.Y))
                                {
                                    callerconvo.Run();

                                    if (callerMenu.Visible)
                                    {
                                        callerMenu.Visible = false;
                                    }
                                    else if (!UIMenu.IsAnyMenuVisible && !TabView.IsAnyPauseMenuVisible)
                                    {
                                        callerMenu.Visible = true;
                                        CalloutInfoMessages.RNUIMenuOpening();
                                    }
                                }
                            }

                            callerconvo.OnConversationEnded += (s, e) =>
                            {
                                currentStage = NoiseComplaintStage.CallerWalksToDoor;
                            };

                            break;

                        case NoiseComplaintStage.CallerWalksToDoor:
                            if (!callerMenu.Visible && Caller.Position.DistanceTo(new Vector3(CallerPos.X, CallerPos.Y, CallerPos.Z)) > 1.0f)
                            {
                                Caller.Tasks.GoStraightToPosition(new Vector3(CallerPos.X, CallerPos.Y, CallerPos.Z), 0.9f, CallerPos.W, 0.5f, 10000);
                                Game.DisplayHelp("Talk to the ~r~Noisy Neighbor~s~", false);

                                calloutBlip = new Blip(new Vector3(NeighborPos.X, NeighborPos.Y, NeighborPos.Z), 10f);
                                calloutBlip.Color = System.Drawing.Color.Red;
                                calloutBlip.Alpha = 0.5f;
                                calloutBlip.Name = "Noisy Neighbor Area";
                                calloutBlip.IsRouteEnabled = true;
                                currentStage = NoiseComplaintStage.WalkToMarker;
                            }
                            break;

                        case NoiseComplaintStage.WalkToMarker:
                            NativeFunction.Natives.DrawMarker(
                                1,
                                MarkerPos.X, MarkerPos.Y, MarkerPos.Z,
                                0f, 0f, 0f,
                                0f, 0f, 0f,
                                1.0f, 1.0f, 1.0f,
                                255, 0, 0, 255,
                                false, true, 2, false, 0, 0, false
                            );

                            if (Game.LocalPlayer.Character.DistanceTo(MarkerPos) < 2f)
                            {
                                if (calloutBlip != null && calloutBlip.Exists())
                                    calloutBlip.Delete();

                                Game.FadeScreenOut(1000);
                                GameFiber.Sleep(1000);
                                currentStage = NoiseComplaintStage.SpawnNeighbor;
                            }
                            break;

                        case NoiseComplaintStage.SpawnNeighbor:
                            Neighbor = new Ped(new Vector3(NeighborPos.X, NeighborPos.Y, NeighborPos.Z), NeighborPos.W);
                            if (!Neighbor.Exists())
                            {
                                CalloutWarnMessages.FailedToCreatePed();
                                End();
                                return;
                            }

                            Neighbor.IsPersistent = true;
                            Neighbor.CanPlayGestureAnimations = false;
                            Neighbor.CanPlayAmbientAnimations = false;
                            NeighborBlip = Neighbor.AttachBlip();
                            NeighborBlip.Color = System.Drawing.Color.Red;
                            NeighborBlip.Name = "Neighbor";

                            PedMoodManager.SetMood(Neighbor);

                            GameFiber.Sleep(500);
                            Game.FadeScreenIn(1000);
                            currentStage = NoiseComplaintStage.TalkToNeighbor;
                            break;

                        case NoiseComplaintStage.TalkToNeighbor:
                            if (Game.LocalPlayer.Character.DistanceTo(Neighbor.Position) < 3f)
                            {
                                Game.DisplayHelp("Press ~y~Y~s~ to talk to the ~r~Noisy Neighbor~s~");

                                if (Game.IsKeyDown(Keys.Y))
                                {
                                    neighborconvo.Run();

                                    if (neighborMenu.Visible)
                                    {
                                        neighborMenu.Visible = false;
                                    }
                                    else if (!UIMenu.IsAnyMenuVisible && !TabView.IsAnyPauseMenuVisible)
                                    {
                                        neighborMenu.Visible = true;
                                        CalloutInfoMessages.RNUIMenuOpening();
                                    }
                                }
                            }
                            break;

                        case NoiseComplaintStage.Done:

                            break;
                    }

                    if (Game.IsKeyDown(Keys.End))
                    {
                        End();
                        return;
                    }

                    GameFiber.Yield();
                }
            }
            catch (Rage.Exceptions.InvalidHandleableException ex)
            {
                CalloutWarnMessages.RageExceptionsInvalidHandleableException(ex.Message);
                End();
            }
            catch (Exception ex)
            {
                CalloutWarnMessages.RageExceptionsUnknown(ex.Message);
                End();
            }
        }

        public override void End()
        {
            base.End();

            if (Caller.Exists())
            {
                Caller.IsPersistent = true;
                Caller.Tasks.Clear();
                Caller.Dismiss();
                CallerBlip.Delete();
            }

            if (Neighbor.Exists())
            {
                Neighbor.IsPersistent = false;
                Neighbor.Tasks.Clear();
                Neighbor.Dismiss();
                NeighborBlip.Delete();
            }

            if (calloutBlip.Exists())
            {
                calloutBlip.Delete();
            }

            if (callerMenu != null)
            {
                MenuManager.Pool.Remove(callerMenu);
                callerMenu = null;
            }

            MenuManager.StopProcessing();
            CalloutInfoMessages.CalloutCleanedUp();

        }
    }
}
