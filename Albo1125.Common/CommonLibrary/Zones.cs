using Rage;
using Rage.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Albo1125.Common.CommonLibrary
{
    public static class Zones
    {
        public enum EWorldZone
        {
            NULL,
            PROL,
            AIRP,
            ALAMO,
            ALTA,
            ARMYB,
            BANHAMC,
            BANNING,
            BEACH,
            BHAMCA,
            BRADP,
            BRADT,
            BURTON,
            CALAFB,
            CANNY,
            CCREAK,
            CHAMH,
            CHIL,
            CHU,
            CMSW,
            CYPRE,
            DAVIS,
            DELBE,
            DELPE,
            DELSOL,
            DESRT,
            DOWNT,
            DTVINE,
            EAST_V,
            EBURO,
            ELGORL,
            ELYSIAN,
            GALFISH,
            golf,
            GRAPES,
            GREATC,
            HARMO,
            HAWICK,
            HORS,
            HUMLAB,
            JAIL,
            KOREAT,
            LACT,
            LAGO,
            LDAM,
            LEGSQU,
            LMESA,
            LOSPUER,
            MIRR,
            MORN,
            MOVIE,
            MTCHIL,
            MTGORDO,
            MTJOSE,
            MURRI,
            NCHU,
            NOOSE,
            OCEANA,
            PALCOV,
            PALETO,
            PALFOR,
            PALHIGH,
            PALMPOW,
            PBLUFF,
            PBOX,
            PROCOB,
            RANCHO,
            RGLEN,
            RICHM,
            ROCKF,
            RTRAK,
            SanAnd,
            SANCHIA,
            SANDY,
            SKID,
            SLAB,
            STAD,
            STRAW,
            TATAMO,
            TERMINA,
            TEXTI,
            TONGVAH,
            TONGVAV,
            VCANA,
            VESP,
            VINE,
            WINDF,
            WVINE,
            ZANCUDO,
            ZP_ORT,
            ZQ_UAR
        }



        public static string GetLowerZoneNameForEntity(this Entity entity)
        {
            return GetZoneName(GetZone(entity.Position)).ToLower();
        }
        public static string GetLowerZoneName(Vector3 pos)
        {
            return GetZoneName(GetZone(pos)).ToLower();
        }
        public static string GetZoneName(Vector3 pos)
        {
            return GetZoneName(GetZone(pos));
        }


        public static EWorldZone GetZone(Vector3 Position)
        {
            string zoneId = NativeFunction.Natives.GET_NAME_OF_ZONE<string>(Position.X, Position.Y, Position.Z);

            EWorldZone result;
            if (Enum.TryParse<EWorldZone>(zoneId, true, out result))
            {
                return result;
            }
            else
            {
                return EWorldZone.NULL;
            }
        }
        private static string[] LosSantosCountyZoneNames = new string[] {"tongva hills", "chumash", "banham canyon drive", "banham canyon", "san andreas", "richman glen", "tongva valley", "great chaparral", "vinewood hills",
        "tataviam mountains", "noose hq", "palomino highlands"};

        public enum WorldDistricts { City, LosSantosCountryside, BlaineCounty, Water }
        public static WorldDistricts GetWorldDistrict(this Vector3 pos)
        {
            if (pos.IsPointOnWater()) { return WorldDistricts.Water; }
            uint zoneHash = Rage.Native.NativeFunction.CallByHash<uint>(0x7ee64d51e8498728, pos.X, pos.Y, pos.Z);


            if (Game.GetHashKey("city") == zoneHash)
            {
                return WorldDistricts.City;
            }
            else if (LosSantosCountyZoneNames.Contains(GetLowerZoneName(pos)))
            {
                return WorldDistricts.LosSantosCountryside;
            }
            else
            {
                return WorldDistricts.BlaineCounty;
            }
        }

        


        public static string GetZoneName(EWorldZone zone)
        {
            switch (zone)
            {
                case EWorldZone.AIRP:
                    return "Los Santos International Airport";
                case EWorldZone.ALAMO:
                    return "The Alamo Sea";
                case EWorldZone.ALTA:
                    return "Alta";
                case EWorldZone.ARMYB:
                    return "Fort Zancudo";
                case EWorldZone.BANHAMC:
                    return "Banham Canyon";
                case EWorldZone.BANNING:
                    return "Banning";
                case EWorldZone.BEACH:
                    return "Vespucci Beach";
                case EWorldZone.BHAMCA:
                    return "Banham Canyon Drive";
                case EWorldZone.BRADP:
                    return "Braddock Pass";
                case EWorldZone.BRADT:
                    return "Braddock Tunnel";
                case EWorldZone.BURTON:
                    return "Burton";
                case EWorldZone.CALAFB:
                    return "Calafia Bridge";
                case EWorldZone.CANNY:
                    return "Raton Canyon";
                case EWorldZone.CCREAK:
                    return "Cassidy Creek";
                case EWorldZone.CHAMH:
                    return "Chamberlain Hills";
                case EWorldZone.CHIL:
                    return "Vinewood Hills";
                case EWorldZone.CHU:
                    return "Chumash";
                case EWorldZone.CMSW:
                    return "Chiliad Mountain State Wilderness";
                case EWorldZone.CYPRE:
                    return "Cypress Flats";
                case EWorldZone.DAVIS:
                    return "Davis";
                case EWorldZone.DELBE:
                    return "Del Perro Beach";
                case EWorldZone.DELPE:
                    return "Del Perro";
                case EWorldZone.DELSOL:
                    return "Puerto Del Sol";
                case EWorldZone.DESRT:
                    return "Grand Senora Desert";
                case EWorldZone.DOWNT:
                    return "Downtown";
                case EWorldZone.DTVINE:
                    return "Downtown Vinewood";
                case EWorldZone.EAST_V:
                    return "East Vinewood";
                case EWorldZone.EBURO:
                    return "El Burro Heights";
                case EWorldZone.ELGORL:
                    return "El Gordo Lighthouse";
                case EWorldZone.ELYSIAN:
                    return "Elysian Island";
                case EWorldZone.GALFISH:
                    return "Galilee";
                case EWorldZone.golf:
                    return "GWC and Golfing Society";
                case EWorldZone.GRAPES:
                    return "Grapeseed";
                case EWorldZone.GREATC:
                    return "Great Chaparral";
                case EWorldZone.HARMO:
                    return "Harmony";
                case EWorldZone.HAWICK:
                    return "Hawick";
                case EWorldZone.HORS:
                    return "Vinewood Racetrack";
                case EWorldZone.HUMLAB:
                    return "Humane Labs and Research";
                case EWorldZone.JAIL:
                    return "Bolingbroke Penitentiary";
                case EWorldZone.KOREAT:
                    return "Little Seoul";
                case EWorldZone.LACT:
                    return "Land Act Reservoir";
                case EWorldZone.LAGO:
                    return "Lago Zancudo";
                case EWorldZone.LDAM:
                    return "Land Act Dam";
                case EWorldZone.LEGSQU:
                    return "Legion Square";
                case EWorldZone.LMESA:
                    return "La Mesa";
                case EWorldZone.LOSPUER:
                    return "La Puerta";
                case EWorldZone.MIRR:
                    return "Mirror Park";
                case EWorldZone.MORN:
                    return "Morningwood";
                case EWorldZone.MOVIE:
                    return "Richards Majestic";
                case EWorldZone.MTCHIL:
                    return "Mount Chiliad";
                case EWorldZone.MTGORDO:
                    return "Mount Gordo";
                case EWorldZone.MTJOSE:
                    return "Mount Josiah";
                case EWorldZone.MURRI:
                    return "Murrieta Heights";
                case EWorldZone.NCHU:
                    return "North Chumash";
                case EWorldZone.NOOSE:
                    return "NOOSE HQ";
                case EWorldZone.OCEANA:
                    return "Pacific Ocean";
                case EWorldZone.PALCOV:
                    return "Paleto Cove";
                case EWorldZone.PALETO:
                    return "Paleto Bay";
                case EWorldZone.PALFOR:
                    return "Paleto Forest";
                case EWorldZone.PALHIGH:
                    return "Palomino Highlands";
                case EWorldZone.PALMPOW:
                    return "Palmer-Taylor Power Station";
                case EWorldZone.PBLUFF:
                    return "Pacific Bluffs";
                case EWorldZone.PBOX:
                    return "Pillbox Hill";
                case EWorldZone.PROCOB:
                    return "Procopio Beach";
                case EWorldZone.RANCHO:
                    return "Rancho";
                case EWorldZone.RGLEN:
                    return "Richman Glen";
                case EWorldZone.RICHM:
                    return "Richman";
                case EWorldZone.ROCKF:
                    return "Rockford Hills";
                case EWorldZone.RTRAK:
                    return "Redwood Lights Track";
                case EWorldZone.SanAnd:
                    return "San Andreas";
                case EWorldZone.SANCHIA:
                    return "San Chianski Mountain Range";
                case EWorldZone.SANDY:
                    return "Sandy Shores";
                case EWorldZone.SKID:
                    return "Mission Row";
                case EWorldZone.SLAB:
                    return "Stab City";
                case EWorldZone.STAD:
                    return "Maze Bank Arena";
                case EWorldZone.STRAW:
                    return "Strawberry";
                case EWorldZone.TATAMO:
                    return "Tataviam Mountains";
                case EWorldZone.TERMINA:
                    return "Terminal";
                case EWorldZone.TEXTI:
                    return "Textile City";
                case EWorldZone.TONGVAH:
                    return "Tongva Hills";
                case EWorldZone.TONGVAV:
                    return "Tongva Valley";
                case EWorldZone.VCANA:
                    return "Vespucci Canals";
                case EWorldZone.VESP:
                    return "Vespucci";
                case EWorldZone.VINE:
                    return "Vinewood";
                case EWorldZone.WINDF:
                    return "RON Alternates Wind Farm";
                case EWorldZone.WVINE:
                    return "West Vinewood";
                case EWorldZone.ZANCUDO:
                    return "Zancudo River";
                case EWorldZone.ZP_ORT:
                    return "Port of South Los Santos";
                case EWorldZone.ZQ_UAR:
                    return "Davis Quartz";
                default:
                    return String.Empty;
            }
        }

    }
}
