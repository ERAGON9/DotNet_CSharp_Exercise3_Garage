﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ex03.GarageLogic
{
    internal abstract class FuelVehicle : Vehicle
    {
        private eFuelType m_FuelType;
        private float m_CurrentFuelAmount;
        private float m_MaxFuelAmount;

        public FuelVehicle(string i_LicensePlate, int i_NumOfWheels, int i_MaxWheelAirPressure,
            eFuelType i_FuelType, float i_MaxFuelAmount)
            : base(i_LicensePlate, i_NumOfWheels, i_MaxWheelAirPressure)
        {
            m_FuelType = i_FuelType;
            m_MaxFuelAmount = i_MaxFuelAmount;
            
        }

        public void FuelCharging(int i_FuelToAdd, eFuelType i_FuelType)
        {
            if (m_FuelType == i_FuelType)
            {
                if (i_FuelToAdd >= 0 && m_CurrentFuelAmount + i_FuelToAdd <= m_MaxFuelAmount)
                {
                    m_CurrentFuelAmount += i_FuelToAdd;
                }
                else
                {
                    throw new ValueOutOfRangeException(m_MaxFuelAmount - m_CurrentFuelAmount, 0);
                }
            }
            else
            {
                throw new ArgumentException("Wrong Fuel Type");
            }
        }

    }
}
