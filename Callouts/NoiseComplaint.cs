using CalloutInterfaceAPI;
using EverydayCallouts.Engine;
using EverydayCallouts.Logging;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EverydayCallouts.Callouts
{
    [CalloutInterface
        ("Noise Complaint", CalloutProbability.Medium, "Caller is reporting a loud party happening in an upstairs appartment.", "Code 1", "LSPD")
    ]

    public class NoiseComplaint : Callout
    {
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

        // Dialog / State Tracking
        private DialogueRunner runner;
        private bool hasCallerApproachedPlayer = false;
        private bool hasTalkedToCaller = false;
        private bool hasDisplayedIntroSubtitle = false;

        // Menu
        private UIMenu callermenu;
        private MenuPool menuPool = new MenuPool();



        private List<(Vector4 SpawnPoint, Vector4 CallerSpawn, Vector4 NeighborSpawn)> _spawnGroups =
            new List<(Vector4, Vector4, Vector4)>
            {
                (new Vector4(-1054.59f, -1003.173f, 1.150192f, 110.9069f),
                new Vector4(-1056.478f, -1000.202f, 1.150192f, 126.4022f),
                new Vector4(-1054.645f, -999.9692f, 5.41049f, 189.0671f))
            };


        public override bool OnBeforeCalloutDisplayed()
        {
            Vector3 playerPos = Game.LocalPlayer.Character.Position;

            float closestDistance = float.MaxValue;
            (Vector4 SpawnPoint, Vector4 CallerSpawn, Vector4 NeighborSpawn) closestGroup = default;

            foreach (var group in _spawnGroups)
            {
                Vector3 spawn = new Vector3(group.SpawnPoint.X, group.SpawnPoint.Y, group.SpawnPoint.Z);
                float distance = playerPos.DistanceTo(spawn);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestGroup = group;
                }
            }

            if (closestDistance > 550f)
            {
                return false;
            }

            SpawnPoint = new Vector3(closestGroup.SpawnPoint.X, closestGroup.SpawnPoint.Y, closestGroup.SpawnPoint.Z);
            CallerPos = closestGroup.CallerSpawn;
            NeighborPos = closestGroup.NeighborSpawn;

            AddMinimumDistanceCheck(100f, SpawnPoint);
            AddMaximumDistanceCheck(550f, SpawnPoint);
            ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 20f);

            LSPD_First_Response.Mod.API.Functions.PlayScannerAudioUsingPosition("WE_HAVE CRIME_CIVIL_DISTURBANCE_01, AREA, STREETS", SpawnPoint);

            CalloutPosition = SpawnPoint;

            return base.OnBeforeCalloutDisplayed();
        }


        public override bool OnCalloutAccepted()
        {
            Caller = new Ped(new Vector3(CallerPos.X, CallerPos.Y, CallerPos.Z), CallerPos.W);
            Caller.IsPersistent = true;
            Caller.CanPlayGestureAnimations = false;
            Caller.CanPlayAmbientAnimations = false;

            if (!Caller.Exists())
            {
                Game.LogTrivial("Failed to create Caller Ped.");
                return false;
            }

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

            callermenu = new UIMenu("Caller", "Speak with the caller");
            MenuManager.Pool.Add(callermenu);

            callermenu.Visible = false;

            runner = new DialogueRunner();
            runner.OnConversationEnd += () =>
            {
                Game.DisplaySubtitle("You finish talking to the caller.");
            };


            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            menuPool.ProcessMenus();

            if (!hasDisplayedIntroSubtitle && Game.LocalPlayer.Character.Position.DistanceTo(Caller.Position) < 30f && Caller.IsAlive)
            {
                Game.DisplaySubtitle("Officer, over here!");
                hasDisplayedIntroSubtitle = true;
                CallerBlip = Caller.AttachBlip();
                CallerBlip.Color = System.Drawing.Color.Yellow;
                CallerBlip.Name = "Caller";
                calloutBlip.Delete();

                CalloutInterfaceAPI.Functions.SendMessage(this, "Officer is making contact with caller");
            }

            else
            {
                string message = CalloutErrorMessages.PedDoesNotExist();
                End();
            }

            if (!hasCallerApproachedPlayer && Game.LocalPlayer.Character.Position.DistanceTo(Caller.Position) < 15f && !Game.LocalPlayer.Character.IsInAnyVehicle(false) && Caller.IsAlive)
            {
                Caller.Tasks.Clear();
                Caller.Tasks.GoToOffsetFromEntity(Game.LocalPlayer.Character, -1, 3.5f, 0f, 0.9f);
                Caller.KeepTasks = true;

                hasCallerApproachedPlayer = true;
                CallerBlip.IsRouteEnabled = false;
            }

            if (!hasTalkedToCaller && Game.LocalPlayer.Character.Position.DistanceTo(Caller.Position) < 3.0f && !Game.LocalPlayer.Character.IsInAnyVehicle(false) && Caller.IsAlive && Caller.IsOnScreen)
            {
                Caller.Tasks.Clear();
                Caller.Tasks.StandStill(-1);
                Game.DisplayHelp("Press ~y~Y~s~ to talk to the caller", false);

                if (Game.IsKeyDown(Keys.Y))
                {
                    runner.StartConversation(callermenu, "dialogue_noise_complaint.json",new Dictionary<string, string>{{ "timeofday", World.TimeOfDay.ToString(@"h:mm tt") }});

                    callermenu.Visible = true;
                    hasTalkedToCaller = true;
                }

            }

            MenuManager.Pool.ProcessMenus();

        }

        public override void End()
        {
            base.End();

            if (Caller.Exists())
            {
                Caller.Tasks.Clear();
                Caller.Dismiss();
                CallerBlip.Delete();
            }

            if (calloutBlip.Exists())
            {
                calloutBlip.Delete();
            }

            if (callermenu != null)
            {
                MenuManager.Pool.Remove(callermenu);
                callermenu = null;
            }

        }
    }

}
