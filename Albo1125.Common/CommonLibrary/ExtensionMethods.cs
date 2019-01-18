using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rage;
using Rage.Native;
using System.Drawing;
using System.Windows.Forms;
using Albo1125.Common.CommonLibrary;
using System.Globalization;
using System.IO;

namespace Albo1125.Common.CommonLibrary
{
    public class TupleList<T1, T2> : List<Tuple<T1, T2>>
    {
        public TupleList(TupleList<T1, T2> tuplelist)
        {
            foreach (Tuple<T1, T2> tuple in tuplelist)
            {
                this.Add(tuple);
            }
        }
        public TupleList() { }
        public void Add(T1 item, T2 item2)
        {
            Add(new Tuple<T1, T2>(item, item2));
        }
    }
    public class TupleList<T1, T2, T3> : List<Tuple<T1, T2, T3>>
    {
        public TupleList() { }
        public TupleList(TupleList<T1, T2, T3> tuplelist)
        {
            foreach (Tuple<T1, T2, T3> tuple in tuplelist)
            {
                this.Add(tuple);
            }
        }
        public void Add(T1 item, T2 item2, T3 item3)
        {
            Add(new Tuple<T1, T2, T3>(item, item2, item3));
        }

    }
    public class TupleList<T1, T2, T3, T4> : List<Tuple<T1, T2, T3, T4>>
    {
        public TupleList() { }
        public TupleList(TupleList<T1, T2, T3, T4> tuplelist)
        {
            foreach (Tuple<T1, T2, T3, T4> tuple in tuplelist)
            {
                this.Add(tuple);
            }
        }
        public void Add(T1 item, T2 item2, T3 item3, T4 item4)
        {
            Add(new Tuple<T1, T2, T3, T4>(item, item2, item3, item4));
        }

    }

    public static class ExtensionMethods
    {


        //private static bool DisplayTime = false;

        public static int[] BlackListedNodeTypes = new int[] { 0, 8, 9, 10, 12, 40, 42, 136 };
        public static int GetNearestNodeType(this Vector3 pos)
        {
            bool get_property_success = false;
            uint node_prop_p1;
            int found_node_type;

            get_property_success = NativeFunction.Natives.GET_VEHICLE_NODE_PROPERTIES<bool>(pos.X, pos.Y, pos.Z, out node_prop_p1, out found_node_type);
            

            if (get_property_success)
            {
                return found_node_type;
            }
            else
            {
                return -1;
            }
        }

        public static bool IsNodeSafe(this Vector3 pos)
        {
            return !BlackListedNodeTypes.Contains(GetNearestNodeType(pos));
        }

        public static bool IsPointOnWater(this Vector3 position)
        {
            float height;

            return NativeFunction.Natives.GET_WATER_HEIGHT<bool>(position.X, position.Y, position.Z, out height);
                      
        }

        public static void DisplayPopupTextBoxWithConfirmation(string Title, string Text, bool PauseGame)
        {
            new Popup(Title, Text, PauseGame, true).Display();
        }   

        public static List<string> WrapText(this string text, double pixels, string fontFamily, float emSize, out double actualHeight)
        {
            string[] originalLines = text.Split(new string[] { " " },
                StringSplitOptions.None);

            List<string> wrappedLines = new List<string>();

            StringBuilder actualLine = new StringBuilder();
            double actualWidth = 0;
            actualHeight = 0;
            foreach (var item in originalLines)
            {
                System.Windows.Media.FormattedText formatted = new System.Windows.Media.FormattedText(item,
                    CultureInfo.CurrentCulture,
                    System.Windows.FlowDirection.LeftToRight,
                    new System.Windows.Media.Typeface(fontFamily), emSize, System.Windows.Media.Brushes.Black);


                actualWidth += formatted.Width;
                actualHeight = formatted.Height;
                
                if (actualWidth > pixels)
                {
                    wrappedLines.Add(actualLine.ToString());
                    actualLine.Clear();
                    actualWidth = 0;
                    actualLine.Append(item + " ");
                    actualWidth += formatted.Width;
                }
                else if (item == Environment.NewLine || item=="\n")
                {
                    wrappedLines.Add(actualLine.ToString());
                    actualLine.Clear();
                    actualWidth = 0;
                }
                else
                {
                    actualLine.Append(item + " ");
                }
            }
            if (actualLine.Length > 0)
                wrappedLines.Add(actualLine.ToString());

            return wrappedLines;
        }

        public static bool IsPolicePed(this Ped ped)
        {
            return ped.RelationshipGroup == "COP";
        }

        public static string GetKeyString(Keys MainKey, Keys ModifierKey)
        {
            if (ModifierKey == Keys.None)
            {
                return MainKey.ToString();
            }
            else
            {
                string strmodKey = ModifierKey.ToString();

                if (strmodKey.EndsWith("ControlKey") | strmodKey.EndsWith("ShiftKey"))
                {
                    strmodKey.Replace("Key", "");
                }

                if (strmodKey.Contains("ControlKey"))
                {
                    strmodKey = "CTRL";
                }
                else if (strmodKey.Contains("ShiftKey"))
                {
                    strmodKey = "Shift";
                }
                else if (strmodKey.Contains("Menu"))
                {
                    strmodKey = "ALT";
                }

                return string.Format("{0} + {1}", strmodKey, MainKey.ToString());
            }
        }

        public static float CalculateHeadingTowardsEntity (this Entity ent, Entity TargetEntity)
        {
            Vector3 directionToTargetEnt = (TargetEntity.Position - ent.Position);
            directionToTargetEnt.Normalize();
            return MathHelper.ConvertDirectionToHeading(directionToTargetEnt);
            
        }

        public static float CalculateHeadingTowardsPosition(this Vector3 start, Vector3 Target)
        {
            Vector3 directionToTargetEnt = (Target - start);
            directionToTargetEnt.Normalize();
            return MathHelper.ConvertDirectionToHeading(directionToTargetEnt);

        }
        public static bool IsKeyDownComputerCheck(Keys KeyPressed)
        {


            if (Rage.Native.NativeFunction.Natives.UPDATE_ONSCREEN_KEYBOARD<int>() != 0)
            {
                
                return Game.IsKeyDown(KeyPressed);
            }
            else
            {
                return false;
            }



        }
        public static bool IsKeyDownRightNowComputerCheck(Keys KeyPressed)
        {


            if (Rage.Native.NativeFunction.Natives.UPDATE_ONSCREEN_KEYBOARD<int>() != 0)
            {
                return Game.IsKeyDownRightNow(KeyPressed);
            }
            else
            {
                return false;
            }



        }

        public static bool IsKeyCombinationDownComputerCheck(Keys MainKey, Keys ModifierKey)
        {
            if (MainKey != Keys.None)
            {
                return ExtensionMethods.IsKeyDownComputerCheck(MainKey) && (ExtensionMethods.IsKeyDownRightNowComputerCheck(ModifierKey)
                || (ModifierKey == Keys.None && !ExtensionMethods.IsKeyDownRightNowComputerCheck(Keys.Shift) && !ExtensionMethods.IsKeyDownRightNowComputerCheck(Keys.Control)
                && !ExtensionMethods.IsKeyDownRightNowComputerCheck(Keys.LControlKey) && !ExtensionMethods.IsKeyDownRightNowComputerCheck(Keys.LShiftKey)));
            }
            else
            {
                return false;
            }
        }
        public static bool IsKeyCombinationDownRightNowComputerCheck(Keys MainKey, Keys ModifierKey)
        {
            if (MainKey != Keys.None)
            {
                return ExtensionMethods.IsKeyDownRightNowComputerCheck(MainKey) && ((ExtensionMethods.IsKeyDownRightNowComputerCheck(ModifierKey)
                    || (ModifierKey == Keys.None && !ExtensionMethods.IsKeyDownRightNowComputerCheck(Keys.Shift) && !ExtensionMethods.IsKeyDownRightNowComputerCheck(Keys.Control)
                    && !ExtensionMethods.IsKeyDownRightNowComputerCheck(Keys.LControlKey) && !ExtensionMethods.IsKeyDownRightNowComputerCheck(Keys.LShiftKey))));
            }
            else
            {
                return false;
            }

        }

        public static string Reverse(this string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public static T PickRandom<T>(this IEnumerable<T> source)
        {
            return source.PickRandom(1).Single();
        }
        public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count)
        {
            return new List<T>(source).Shuffle().Take(count);
        }
        public static List<T> Shuffle<T>(this List<T> List)
        {
            List<T> list = new List<T>(List);
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = CommonVariables.rnd.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
            
        }

        public static void MakeMissionPed(this Ped ped)
        {
            ped.BlockPermanentEvents = true;
            ped.IsPersistent = true;

        }
        public static Ped ClonePed(this Ped oldPed)
        {
            Vector3 oldPedPosition = oldPed.Position;
            float oldPedHeading = oldPed.Heading;
            bool spawnInVehicle = false;
            Vehicle car = null;
            int seatindex = 0;
            int oldarmor = oldPed.Armor;
            int oldhealth = oldPed.Health;
            if (oldPed.IsInAnyVehicle(false))
            {
                car = oldPed.CurrentVehicle;
                seatindex = oldPed.SeatIndex;
                spawnInVehicle = true;
            }
            Ped newPed = NativeFunction.Natives.ClonePed<Ped>(oldPed, oldPed.Heading, false, true);
            if (oldPed.Exists() && oldPed.IsValid())
            {
                oldPed.Delete();
            }
            newPed.Position = oldPedPosition;
            newPed.Heading = oldPedHeading;

            if (spawnInVehicle)
            {
                newPed.WarpIntoVehicle(car, seatindex);
            }
            newPed.Health = oldhealth;
            newPed.Armor = oldarmor;
            newPed.BlockPermanentEvents = true;
            newPed.IsPersistent = true;
            return newPed;
        }

        /// <summary>
        /// Toggles the neon light in a vehicle
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="neonLight">Neon index</param>
        /// <param name="toggle">Toggle the neon</param>
        public static void ToggleNeonLight(this Vehicle vehicle, ENeonLights neonLight, bool toggle)
        {
            ulong SetVehicleNeonLightEnabledHash = 0x2aa720e4287bf269;

            NativeFunction.CallByHash<uint>(SetVehicleNeonLightEnabledHash, vehicle, (int)neonLight, toggle);
        }


        /// <summary>
        /// Sets the neon light color
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="color">Color to set</param>
        public static void SetNeonLightsColor(this Vehicle vehicle, Color color)
        {
            ulong SetVehicleNeonLightsColoursHash = 0x8e0a582209a62695;

            NativeFunction.CallByHash<uint>(SetVehicleNeonLightsColoursHash, vehicle, (int)color.R, (int)color.G, (int)color.B);
        }


        /// <summary>
        /// Returns true if the neon light is enabled
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="neonLight">Neon index</param>
        /// <returns>true if the neon light is enabled</returns>
        public static bool IsNeonLightEnable(this Vehicle vehicle, ENeonLights neonLight)
        {
            ulong IsVehicleNeonLightEnabledHash = 0x8c4b92553e4766a5;
            if (NativeFunction.CallByHash<bool>(IsVehicleNeonLightEnabledHash, vehicle, (int)neonLight)) return true;
            else if (!NativeFunction.CallByHash<bool>(IsVehicleNeonLightEnabledHash, vehicle, (int)neonLight)) return false;
            else return false;
        }


        /// <summary>
        /// Returns the neon light color
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns>the neon light color</returns>
        public static System.Drawing.Color GetNeonLightsColor(this Vehicle vehicle)
        {
            return UnsafeGetNeonLightsColor(vehicle);
        }
        private static unsafe System.Drawing.Color UnsafeGetNeonLightsColor(Vehicle vehicle)
        {
            Color color;
            int red;
            int green;
            int blue;
            ulong GetVehicleNeonLightsColourHash = 0x7619eee8c886757f;
            NativeFunction.CallByHash<uint>(GetVehicleNeonLightsColourHash, vehicle, &red, &green, &blue);

            return color = Color.FromArgb(red, green, blue);
        }



        /// <summary>
        /// Gets the primary and secondary colors of this instance of Rage.Vehicle
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static VehicleColor GetColors(this Vehicle v)
        {
            return UnsafeGetVehicleColors(v);
        }

        private static unsafe VehicleColor UnsafeGetVehicleColors(Vehicle vehicle)
        {
            int colorPrimaryInt;
            int colorSecondaryInt;

            ulong GetVehicleColorsHash = 0xa19435f193e081ac;
            NativeFunction.CallByHash<uint>(GetVehicleColorsHash, vehicle, &colorPrimaryInt, &colorSecondaryInt);

            VehicleColor colors = new VehicleColor();

            colors.PrimaryColor = (EPaint)colorPrimaryInt;
            colors.SecondaryColor = (EPaint)colorSecondaryInt;

            return colors;
        }



        /// <summary>
        /// Sets the color to this Rage.Vehicle instance
        /// </summary>
        /// <param name="v"></param>
        /// <param name="primaryColor">The primary color</param>
        /// <param name="secondaryColor">The secondary color</param>
        public static void SetColors(this Vehicle v, EPaint primaryColor, EPaint secondaryColor)
        {
            NativeFunction.Natives.SET_VEHICLE_COLOURS(v, (int)primaryColor, (int)secondaryColor);
        }
        /// <summary>
        /// Sets the color to this Rage.Vehicle instance
        /// </summary>
        /// <param name="v"></param>
        /// <param name="color">The color</param>
        public static void SetColors(this Vehicle v, VehicleColor color)
        {
            NativeFunction.Natives.SET_VEHICLE_COLOURS(v, (int)color.PrimaryColor, (int)color.SecondaryColor);
        }

        /// <summary>
        /// Randomise the license plate to avoid excessively frequent debug plates from showing.
        /// </summary>
        public static void RandomiseLicencePlate(this Vehicle vehicle)
        {
            if (vehicle)
            {
                vehicle.LicensePlate = MathHelper.GetRandomInteger(9).ToString() +
                                       MathHelper.GetRandomInteger(9).ToString() +
                                       Convert.ToChar(MathHelper.GetRandomInteger(0, 25) + 65) +
                                       Convert.ToChar(MathHelper.GetRandomInteger(0, 25) + 65) +
                                       Convert.ToChar(MathHelper.GetRandomInteger(0, 25) + 65) +
                                       MathHelper.GetRandomInteger(9).ToString() +
                                       MathHelper.GetRandomInteger(9).ToString() +
                                       MathHelper.GetRandomInteger(9).ToString();
#if DEBUG
                Game.LogTrivial($"Set {vehicle.Model.Name} license plate to {vehicle.LicensePlate}");
#endif
            }
        }

        /// Cache the result of whether a vehicle is an ELS vehicle.
        /// </summary>
        private static Dictionary<Model, bool> vehicleModelELSCache = new Dictionary<Model, bool>();

        /// <summary>
        /// Determine whether the passed vehicle model is an ELS vehicle.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool VehicleModelIsELS(Model model)
        {
            try
            {
                if (vehicleModelELSCache.ContainsKey(model))
                {
                    return vehicleModelELSCache[model];
                }

                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "ELS")))
                {
                    // no ELS installation at all
                    vehicleModelELSCache.Add(model, false);
                    return false;
                }

                IEnumerable<string> elsFiles = Directory.EnumerateFiles(
                    Path.Combine(Directory.GetCurrentDirectory(), "ELS"),
                    $"{model.Name}.xml", SearchOption.AllDirectories);

                vehicleModelELSCache.Add(model, elsFiles.Any());
                return vehicleModelELSCache[model];
            }
            catch (Exception e)
            {
                Game.LogTrivial($"Failed to determine if a vehicle model '{model}' was ELS-enabled: {e}");
                return false;
            }
        }

        /// <summary>
        /// Determine whether the passed vehicle is an ELS vehicle.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public static bool VehicleModelIsELS(this Vehicle vehicle)
        {
            if (vehicle)
            {
                return VehicleModelIsELS(vehicle.Model);
            }
            return false;
        }

    }


    public enum ENeonLights
    {
        Front = 2,
        Back = 3,
        Left = 0,
        Right = 1,
    }

    public enum EPaint
    {
        /* CLASSIC|METALLIC */
        Black = 0,
        Carbon_Black = 147,
        Graphite = 1,
        Anhracite_Black = 11,
        Black_Steel = 2,
        Dark_Steel = 3,
        Silver = 4,
        Bluish_Silver = 5,
        Rolled_Steel = 6,
        Shadow_Silver = 7,
        Stone_Silver = 8,
        Midnight_Silver = 9,
        Cast_Iron_Silver = 10,
        Red = 27,
        Torino_Red = 28,
        Formula_Red = 29,
        Lava_Red = 150,
        Blaze_Red = 30,
        Grace_Red = 31,
        Garnet_Red = 32,
        Sunset_Red = 33,
        Cabernet_Red = 34,
        Wine_Red = 143,
        Candy_Red = 35,
        Hot_Pink = 135,
        Pfister_Pink = 137,
        Salmon_Pink = 136,
        Sunrise_Orange = 36,
        Orange = 38,
        Bright_Orange = 138,
        Gold = 37,
        Bronze = 90,
        Yellow = 88,
        Race_Yellow = 89,
        Dew_Yellow = 91,
        Green = 139,
        Dark_Green = 49,
        Racing_Green = 50,
        Sea_Green = 51,
        Olive_Green = 52,
        Bright_Green = 53,
        Gasoline_Green = 54,
        Lime_Green = 92,
        Hunter_Green = 144,
        Securiror_Green = 125,
        Midnight_Blue = 141,
        Galaxy_Blue = 61,
        Dark_Blue = 62,
        Saxon_Blue = 63,
        Blue = 64,
        Bright_Blue = 140,
        Mariner_Blue = 65,
        Harbor_Blue = 66,
        Diamond_Blue = 67,
        Surf_Blue = 68,
        Nautical_Blue = 69,
        Racing_Blue = 73,
        Ultra_Blue = 70,
        Light_Blue = 74,
        Police_Car_Blue = 127,
        Epsilon_Blue = 157,
        Chocolate_Brown = 96,
        Bison_Brown = 101,
        Creek_Brown = 95,
        Feltzer_Brown = 94,
        Maple_Brown = 97,
        Beechwood_Brown = 103,
        Sienna_Brown = 104,
        Saddle_Brown = 98,
        Moss_Brown = 100,
        Woodbeech_Brown = 102,
        Straw_Brown = 99,
        Sandy_Brown = 105,
        Bleached_Brown = 106,
        Schafter_Purple = 71,
        Spinnaker_Purple = 72,
        Midnight_Purple = 142,
        Metallic_Midnight_Purple = 146,
        Bright_Purple = 145,
        Cream = 107,
        Ice_White = 111,
        Frost_White = 112,
        Pure_White = 134,
        Default_Alloy = 156,
        Champagne = 93,

        /* MATTE */
        Matte_Black = 12,
        Matte_Gray = 13,
        Matte_Light_Gray = 14,
        Matte_Ice_White = 131,
        Matte_Blue = 83,
        Matte_Dark_Blue = 82,
        Matte_Midnight_Blue = 84,
        Matte_Midnight_Purple = 149,
        Matte_Schafter_Purple = 148,
        Matte_Red = 39,
        Matte_Dark_Red = 40,
        Matte_Orange = 41,
        Matte_Yellow = 42,
        Matte_Lime_Green = 55,
        Matte_Green = 128,
        Matte_Forest_Green = 151,
        Matte_Foliage_Green = 155,
        Matte_Brown = 129,
        Matte_Olive_Darb = 152,
        Matte_Dark_Earth = 153,
        Matte_Desert_Tan = 154,

        /* Util */
        Util_Black = 15,
        Util_Black_Poly = 16,
        Util_Dark_Silver = 17,
        Util_Silver = 18,
        Util_Gun_Metal = 19,
        Util_Shadow_Silver = 20,
        Util_Red = 43,
        Util_Bright_Red = 44,
        Util_Garnet_Red = 45,
        Util_Dark_Green = 56,
        Util_Green = 57,
        Util_Dark_Blue = 75,
        Util_Midnight_Blue = 76,
        Util_Blue = 77,
        Util_Sea_Foam_Blue = 78,
        Util_Lightning_Blue = 79,
        Util_Maui_Blue_Poly = 80,
        Util_Bright_Blue = 81,
        Util_Brown = 108,
        Util_Medium_Brown = 109,
        Util_Light_Brown = 110,
        Util_Off_White = 122,

        /* Worn */
        Worn_Black = 21,
        Worn_Graphite = 22,
        Worn_Silver_Grey = 23,
        Worn_Silver = 24,
        Worn_Blue_Silver = 25,
        Worn_Shadow_Silver = 26,
        Worn_Red = 46,
        Worn_Golden_Red = 47,
        Worn_Dark_Red = 48,
        Worn_Dark_Green = 58,
        Worn_Green = 59,
        Worn_Sea_Wash = 60,
        Worn_Dark_Blue = 85,
        Worn_Blue = 86,
        Worn_Light_Blue = 87,
        Worn_Honey_Beige = 113,
        Worn_Brown = 114,
        Worn_Dark_Brown = 115,
        Worn_Straw_Beige = 116,
        Worn_Off_White = 121,
        Worn_Yellow = 123,
        Worn_Light_Orange = 124,
        Worn_Taxi_Yellow = 126,
        Worn_Orange = 130,
        Worn_White = 132,
        Worn_Olive_Army_Green = 133,

        /* METALS */
        Brushed_Steel = 117,
        Brushed_Black_Steel = 118,
        Brushed_Aluminum = 119,
        Pure_Gold = 158,
        Brushed_Gold = 159,
        Secret_Gold = 160,

        /* CHROME */
        Chrome = 120,
    }

}
public struct VehicleColor
{
    /// <summary>
    /// The primary color paint index 
    /// </summary>
    public EPaint PrimaryColor { get; set; }

    /// <summary>
    /// The secondary color paint index 
    /// </summary>
    public EPaint SecondaryColor { get; set; }



    /// <summary>
    /// Gets the primary color name
    /// </summary>
    public string PrimaryColorName
    {
        get { return GetColorName(PrimaryColor); }
    }
    /// <summary>
    /// Gets the secondary color name
    /// </summary>
    public string SecondaryColorName
    {
        get { return GetColorName(SecondaryColor); }
    }



    /// <summary>
    /// Gets the color name
    /// </summary>
    /// <param name="paint">Color to get the name from</param>
    /// <returns></returns>
    public string GetColorName(EPaint paint)
    {
        String name = Enum.GetName(typeof(EPaint), paint);
        return name.Replace("_", " ");
    }
}

