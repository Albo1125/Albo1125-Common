using Rage;
using Rage.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Albo1125.Common.CommonLibrary
{
    public class SpawnPoint
    {
        public Vector3 Position = Vector3.Zero;
        public float Heading = 0;

        public SpawnPoint() { }
        public SpawnPoint(Vector3 Position, float Heading)
        {
            this.Position = Position;
            this.Heading = Heading;
        }

        public static implicit operator Vector3(SpawnPoint s)
        {
            return s.Position;
        }

        public static implicit operator float(SpawnPoint s)
        {
            return s.Heading;
        }

    }

    public static class SpawnPointExtensions
    {
        public static Vector3 GetClosestMajorVehicleNode(this Vector3 startPoint)
        {
            Vector3 ClosestMajorVehicleNode;

            Rage.Native.NativeFunction.Natives.GET_CLOSEST_MAJOR_VEHICLE_NODE<bool>(startPoint.X, startPoint.Y, startPoint.Z, out ClosestMajorVehicleNode, 3.0f, 0f);
            return ClosestMajorVehicleNode;
            
        }

        public static unsafe bool GetSafeVector3ForPed(this Vector3 StartPoint, out Vector3 SafePedPoint)
        {
            Vector3 tempspawn;
            if (!NativeFunction.Natives.GET_SAFE_COORD_FOR_PED<bool>(StartPoint.X, StartPoint.Y, StartPoint.Z, true, out tempspawn, 0))
            {
                tempspawn = World.GetNextPositionOnStreet(StartPoint);
                Entity nearbyentity = World.GetClosestEntity(tempspawn, 25f, GetEntitiesFlags.ConsiderHumanPeds);
                if (nearbyentity.Exists())
                {
                    tempspawn = nearbyentity.Position;
                    SafePedPoint = tempspawn;
                    return true;
                }
                else
                {
                    SafePedPoint = tempspawn;
                    return false;
                }
            }
            SafePedPoint = tempspawn;
            return true;
        }

        public static bool GetClosestVehicleSpawnPoint(this Vector3 SearchPoint, out SpawnPoint sp)
        {
            Vector3 TempSpawnPoint;
            float TempHeading;
            bool GuaranteedSpawnPointFound = true;
            unsafe
            {
                if (!NativeFunction.Natives.GET_CLOSEST_VEHICLE_NODE_WITH_HEADING<bool>(SearchPoint.X, SearchPoint.Y, SearchPoint.Z, out TempSpawnPoint, out TempHeading, 1, 0x40400000, 0) || !Albo1125.Common.CommonLibrary.ExtensionMethods.IsNodeSafe(TempSpawnPoint))
                {
                    TempSpawnPoint = World.GetNextPositionOnStreet(SearchPoint);

                    Entity closestent = World.GetClosestEntity(TempSpawnPoint, 30f, GetEntitiesFlags.ConsiderGroundVehicles | GetEntitiesFlags.ExcludeEmptyVehicles | GetEntitiesFlags.ExcludePlayerVehicle);
                    if (closestent.Exists())
                    {
                        TempSpawnPoint = closestent.Position;
                        TempHeading = closestent.Heading;
                        closestent.Delete();
                    }
                    else
                    {
                        Vector3 directionFromSpawnToPlayer = (Game.LocalPlayer.Character.Position - TempSpawnPoint);
                        directionFromSpawnToPlayer.Normalize();

                        TempHeading = MathHelper.ConvertDirectionToHeading(directionFromSpawnToPlayer) + 180f;
                        GuaranteedSpawnPointFound = false;
                    }
                }
            }
            sp = new SpawnPoint(TempSpawnPoint, TempHeading);
            return GuaranteedSpawnPointFound;
        }


        public static bool GetVehicleSpawnPointTowardsStartPoint(this Vector3 StartPoint, float SpawnDistance, bool UseSpecialID, out SpawnPoint sp)
        {
            Vector3 tempspawn = World.GetNextPositionOnStreet(StartPoint.Around2D(SpawnDistance + 5f));
            Vector3 spawnPoint = Vector3.Zero;
            float Heading = 0;
            bool specialIDused = true;
            if (!UseSpecialID || !NativeFunction.Natives.GET_NTH_CLOSEST_VEHICLE_NODE_FAVOUR_DIRECTION<bool>(tempspawn.X, tempspawn.Y, tempspawn.Z, StartPoint.X, StartPoint.Y, StartPoint.Z, 0, out spawnPoint, out Heading, 0, 0x40400000, 0) || !ExtensionMethods.IsNodeSafe(spawnPoint))
            {
                spawnPoint = World.GetNextPositionOnStreet(StartPoint.Around2D(SpawnDistance + 5f));
                Vector3 directionFromVehicleToPed1 = (StartPoint - spawnPoint);
                directionFromVehicleToPed1.Normalize();

                Heading = MathHelper.ConvertDirectionToHeading(directionFromVehicleToPed1);
                specialIDused = false;
            }
            
            sp = new SpawnPoint(spawnPoint, Heading);
            return specialIDused;
        }

        public static SpawnPoint GetVehicleSpawnPointTowardsPositionWithChecks(this Vector3 StartPoint, float SpawnDistance)
        {
            SpawnPoint sp = new SpawnPoint();
            bool UseSpecialID = true;
            float travelDistance;
            int waitCount = 0;
            while (true)
            {
                GetVehicleSpawnPointTowardsStartPoint(StartPoint, SpawnDistance, UseSpecialID, out sp);
                travelDistance = Rage.Native.NativeFunction.Natives.CALCULATE_TRAVEL_DISTANCE_BETWEEN_POINTS<float>(sp.Position.X, sp.Position.Y, sp.Position.Z, StartPoint.X, StartPoint.Y, StartPoint.Z);
                waitCount++;
                if (Vector3.Distance(StartPoint, sp) > SpawnDistance - 15f)
                {

                    if (travelDistance < (SpawnDistance * 4.5f))
                    {

                        Vector3 directionFromVehicleToPed1 = (StartPoint - sp.Position);
                        directionFromVehicleToPed1.Normalize();

                        float HeadingToPlayer = MathHelper.ConvertDirectionToHeading(directionFromVehicleToPed1);

                        if (Math.Abs(MathHelper.NormalizeHeading(sp.Heading) - MathHelper.NormalizeHeading(HeadingToPlayer)) < 150f)
                        {
                            break;
                        }
                    }
                }
                if (waitCount >= 400)
                {
                    UseSpecialID = false;
                }
                if (waitCount == 600)
                {
                    Game.DisplayNotification("Press ~b~Y ~s~to force a spawn in the ~g~wilderness.");
                }
                if ((waitCount >= 600) && Albo1125.Common.CommonLibrary.ExtensionMethods.IsKeyDownComputerCheck(Keys.Y))
                {
                    return new SpawnPoint(Game.LocalPlayer.Character.Position.Around2D(20f), 0);
                }

                GameFiber.Yield();
            }
            return sp;
        }
    }
}
