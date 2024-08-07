﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// $G$ DSN-001 (-0) Code duplication. except in energy type, gas and electric car are identical.

namespace Ex03.GarageLogic
{
    internal class FuelCar : Car
    {
        private const float k_MaxFuelAmount = 45f;

        public FuelCar(string i_LicensePlate) :base(i_LicensePlate)
        {
            m_Engine = new FuelEngine(k_MaxFuelAmount, FuelEngine.eFuelType.Octan95);
            AddRequirements();
        }

        protected override void AddRequirements()
        {
            base.AddRequirements();
        }

        public override void UpdateStateByRequirements()
        {
            base.UpdateStateByRequirements();
        }

        public override string ToString()
        {
            string stringToReturn = string.Format("vehicle type: fuel car" + 
                Environment.NewLine + "{0}", base.ToString());

            return stringToReturn;
        }
    }
}
