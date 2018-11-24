using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Engine.Scripting.Entities;

namespace Albo1125.Common.CommonLibrary
{
    public static class ExtensionMethodsLSPDFR
    {
        public static Ped ClonePed(this Ped oldped, bool ClonePersona)
        {
            Persona oldpers = Functions.GetPersonaForPed(oldped);
            Ped NewPed = oldped.ClonePed();
            if (ClonePersona)
            {
                Functions.SetPersonaForPed(NewPed, oldpers);

            }
            return NewPed;
        }


    }
}
