using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using System;
using System.Collections.Generic;

namespace EverydayCallouts.Callouts
{
    [CalloutInfo("Noise Complaint", CalloutProbability.High)]

    public class NoiseComplaint : Callout
    {
        private Ped Caller;
        private int dialogCounter = 0;
        private bool isNearCaller = false;
        private bool hasCallerApproachedPlayer = false;
        private bool hasTalkedToCaller = false;
        private bool hasDisplayedIntroSubtitle = false;
        private Ped Neighbor;
        private Blip CallerBlip;
        private Blip NeighborBlip;
        private Vector3 SpawnPoint;
        private Vector4 CallerPos;
        private Vector4 NeighborPos;
        private List<(Vector4 SpawnPoint, Vector4 CallerSpawn, Vector4 NeighborSpawn)> _spawnGroups =
            new List<(Vector4, Vector4, Vector4)>
            {
                (new Vector4(-1054.59f, -1003.173f, 1.150192f, 110.9069f), // SpawnPoint
                new Vector4(-1056.478f, -1000.202f, 1.150192f, 126.4022f), // CallerSpawn
                new Vector4(-1054.645f, -999.9692f, 5.41049f, 189.0671f)) // NeihborSpawn
            };


        public override bool OnBeforeCalloutDisplayed()
        {
            Vector3 playerPos = Game.LocalPlayer.Character.Position;

            // Find the closest spawn group to the player
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

            // Don't run the callout if it's too far from the player
            if (closestDistance > 550f)
            {
                return false;
            }

            // Otherwise, continue with setup
            SpawnPoint = new Vector3(closestGroup.SpawnPoint.X, closestGroup.SpawnPoint.Y, closestGroup.SpawnPoint.Z);
            CallerPos = closestGroup.CallerSpawn;
            NeighborPos = closestGroup.NeighborSpawn;

            AddMinimumDistanceCheck(100f, SpawnPoint);
            AddMaximumDistanceCheck(550f, SpawnPoint);
            ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 20f);

            Functions.PlayScannerAudioUsingPosition("WE_HAVE CRIME_CIVIL_DISTURBANCE_01, AREA, STREETS", SpawnPoint);
            CalloutMessage = "Noise Complaint";
            CalloutPosition = SpawnPoint;

            return base.OnBeforeCalloutDisplayed();
        }


        public override bool OnCalloutAccepted()
        {
            Caller = new Ped(new Vector3(CallerPos.X, CallerPos.Y, CallerPos.Z), CallerPos.W);
            Caller.IsPersistent = true;
            Caller.CanBeTargetted = false;
            Caller.CanPlayGestureAnimations = false;
            Caller.CanPlayAmbientAnimations = false;

            Caller.Tasks.PlayAnimation(
                "gestures@m@standing@casual",
                "gesture_hello",
                1.0f,
                AnimationFlags.Loop | AnimationFlags.UpperBodyOnly
            );

            // Create the blips for the caller and neighbor
            CallerBlip = Caller.AttachBlip();
            CallerBlip.Color = System.Drawing.Color.Yellow;
            CallerBlip.Name = "Caller";
            CallerBlip.IsRouteEnabled = true;
          
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (!hasTalkedToCaller && Game.LocalPlayer.Character.Position.DistanceTo(Caller.Position) < 3.0f && !Game.LocalPlayer.Character.IsInAnyVehicle(false))
            {
                Game.DisplayHelp("Press ~y~Y~s~ to talk to the caller", false);

                if (!hasDisplayedIntroSubtitle)
                {
                    Game.DisplaySubtitle("Officer, over here!");
                    hasDisplayedIntroSubtitle = true;
                }

                if (Game.IsKeyDown(System.Windows.Forms.Keys.Y))
                {
                    Caller.Tasks.Clear();
                    Caller.Tasks.AchieveHeading(Game.LocalPlayer.Character.Heading);

                    dialogCounter++;

                    if (dialogCounter == 1)
                    {
                        Game.DisplaySubtitle("Hi. We got a call to this address for a noise complaint? Are you the caller?");
                    }
                    else if (dialogCounter == 2)
                    {
                        Game.DisplaySubtitle("Caller: Yes officer, it's been insanely loud all night. My baby won't go to sleep with their party upstairs!");
                    }
                    else if (dialogCounter == 3)
                    {
                        Game.DisplaySubtitle("Alright, I’ll check it out. Please stay here. I'll find you if I need anything else.");
                    }
                    else if (dialogCounter > 3)
                    {
                        Game.DisplayHelp("Talk to the Neighbor upstairs");
                        hasTalkedToCaller = true;
                    }
                }
            }
        }



        public override void End()
        {
            // Logic to clean up after the callout is finished
            base.End();

            if (Caller.Exists())
            {
                Caller.Dismiss();
                CallerBlip.Delete();
            }
        }
    }

}
