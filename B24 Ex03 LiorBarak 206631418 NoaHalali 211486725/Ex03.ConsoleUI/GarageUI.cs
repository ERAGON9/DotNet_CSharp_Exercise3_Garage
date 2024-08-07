﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Ex03.GarageLogic;

namespace Ex03.ConsoleUI
{
    internal class GarageUI
    {

        // $G$ DSN-009 (-0) You should separate different classes/enums into different files.
        private enum eUserAction
        {
            InsertNewVehicle = 1,
            DisplayLicensesPlatesList = 2,
            ChangeVehicleGarageState = 3,
            FillWheelsWithAir = 4,
            ChargeFuelVehicle = 5,
            ChargeElectricVehicle = 6,
            DisplayClientData = 7,
        }

        // $G$ DSN-009 (-0) You should separate different classes/enums into different files.
        private enum eUIGarageStateFilter
        {
            All = 1,
            InRepair = 2,
            Repaired = 3,
            Paid = 4,
        }

        private GarageSystem m_GarageSystem = new GarageSystem();
        private MessagesUI m_MessagesToPrint = new MessagesUI();
        private bool m_ProgramStillRunning = true;

        public void RunSystem()
        {
            m_MessagesToPrint.WelcomeMessage();
            while (m_ProgramStillRunning)
            {
                m_MessagesToPrint.MenuMessage();
                eUserAction userOption = getActionOptionFromUser();
                Console.Clear();
                activateAction(userOption);
                Console.WriteLine();
            }
        }

        // $G$ DSN-012 (-0) Unnecessary code duplication, the difference between the methods is too small.
        private eUserAction getActionOptionFromUser()
        {
            const int k_MinOptionVal = 1;
            const int k_MaxOptionVal = 7;
            bool isValid = false;
            string userInput;
            eUserAction? userOptionNumber = null;

            while (!isValid)
            {
                userInput = Console.ReadLine();
                isValid = Enum.TryParse(userInput, out eUserAction enumOptionNumber);
                if (isValid)
                {
                    userOptionNumber = enumOptionNumber;
                    break;
                }

                Console.WriteLine("Input must be between {0} to {1}, try again.",
                k_MinOptionVal, k_MaxOptionVal);
            }

            return userOptionNumber.Value;
        }

        private void activateAction(eUserAction i_ActionNumber)
        {
            switch (i_ActionNumber)
            {
                case eUserAction.InsertNewVehicle:
                    {
                        insertNewVehicleToGarage();
                        break;
                    }
                case eUserAction.DisplayLicensesPlatesList:
                    {
                        displayLicensesPlatesList();
                        break;
                    }
                case eUserAction.ChangeVehicleGarageState:
                    {
                        changeVehicleGarageState();
                        break;
                    }
                case eUserAction.FillWheelsWithAir:
                    {
                        fillWheelsWithAirToMaximum();
                        break;
                    }
                case eUserAction.ChargeFuelVehicle:
                    {
                        chargeFuelVehicle();
                        break;
                    }
                case eUserAction.ChargeElectricVehicle:
                    {
                        chargeElectricVehicle();
                        break;
                    }
                case eUserAction.DisplayClientData:
                    {
                        displayClientData();
                        break;
                    }
            }
        }

        private void insertNewVehicleToGarage()
        {
            try
            {
                string licensePlate = getLicensePlateFromUser();
                bool existingClient = m_GarageSystem.IsVehicleAlreadyExistsAtGarage(licensePlate);

                if (existingClient)
                {
                    Console.WriteLine("The vehicle already been at the garage before.");
                    Console.WriteLine("Vehicle state at the garage changing to: in repair.");
                    m_GarageSystem.ChangeVehicleState(licensePlate, eVehicleGarageState.InRepair);
                }
                else
                {
                    string vehicleType = chooseVehicleType();
                    Vehicle newVehicle = VehicleFactory.CreateNewVehicle(vehicleType, licensePlate);
                    setVehicleState(newVehicle);
                    addClientToGarageSystem(newVehicle);
                    Console.WriteLine("Inserted new vehicle to the garage system successfully.");
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Error: {0}, try again.", ex.Message);
                insertNewVehicleToGarage();
            }
            catch (FormatException ex)
            {
                Console.WriteLine("Error: {0}, try again.", ex.Message);
                insertNewVehicleToGarage();
            }
            catch (ValueOutOfRangeException ex)
            {
                Console.WriteLine("Error: {0}, need to be between {1} to {2}, try again.",
                    ex.Message, ex.MinValue, ex.MaxValue);
                insertNewVehicleToGarage();
            }
        }

        private string getLicensePlateFromUser()
        {
            Console.Write("Please enter vehicle license plate: ");
            string licensePlate = Console.ReadLine();

            return licensePlate;
        }

        private string chooseVehicleType()
        {
            m_MessagesToPrint.VehiclesOptionsMessage();
            Console.Write("Your choice: ");
            string vehicleType = Console.ReadLine();

            return vehicleType;
        }

        private void setVehicleState(Vehicle i_NewVehicle)
        {
            sameWheelsStateOption(i_NewVehicle);
            Dictionary<string, string> NewVehicleRequirements = i_NewVehicle.Requirements;
            List<string> keys = new List<string>(NewVehicleRequirements.Keys);
            string value;

            foreach (string requirement in keys)
            {
                Console.WriteLine("Please enter {0}:", requirement.ToLower());
                value = Console.ReadLine();
                NewVehicleRequirements[requirement] = value;
            }

            i_NewVehicle.UpdateStateByRequirements();
        }

        private void sameWheelsStateOption(Vehicle i_NewVehicle)
        {
            Console.WriteLine("Do you want to enter same state for all the vehicle wheels? (yes/no)");
            string sameWheelState = Console.ReadLine();

            if (sameWheelState == "yes")
            {
                i_NewVehicle.SameWheelsStateAddRequirements();
            }
        }

        private void addClientToGarageSystem(Vehicle i_Vehicle)
        {
            Console.WriteLine("Please enter client name:");
            string clientName = Console.ReadLine();
            Console.WriteLine("Please enter client phone number:");
            string clientPhoneNumber = Console.ReadLine();
            m_GarageSystem.AddNewClientToGarageSystem(clientName, clientPhoneNumber, i_Vehicle);
        }

        private void displayLicensesPlatesList()
        {
            List<string> LicensesPlatesList;
            eUIGarageStateFilter filter = getFilterOptionFromUser();

            LicensesPlatesList = getLicensesPlatesListByFilterOption(filter);
            foreach (string licensePlate in LicensesPlatesList)
            {
                Console.WriteLine(licensePlate);
            }

            if (LicensesPlatesList.Count == 0)
            {
                Console.WriteLine("No licenses plates at this filter option.");
            }
        }

        // $G$ DSN-012 (-5) Unnecessary code duplication, the difference between the methods is too small.
        private eUIGarageStateFilter getFilterOptionFromUser()
        {
            const int k_MinOptionVal = 1;
            const int k_MaxOptionVal = 4;
            bool isValid = false;
            string userInput;
            eUIGarageStateFilter? userOptionNumber = null;

            m_MessagesToPrint.FilterOptionsMessage();
            while (!isValid)
            {
                userInput = Console.ReadLine();
                isValid = Enum.TryParse(userInput, out eUIGarageStateFilter enumOptionNumber);
                if (isValid)
                {
                    userOptionNumber = enumOptionNumber;
                    break;
                }

                Console.WriteLine("Input must be between {0} to {1}, try again.",
                k_MinOptionVal, k_MaxOptionVal);
            }

            return userOptionNumber.Value;
        }

        private List<string> getLicensesPlatesListByFilterOption(eUIGarageStateFilter i_Filter)
        {
            List<string> LicensesPlatesList = null; //will alway be one of the cases here,
                                                    //filter validation is being checked before

            switch (i_Filter)
            {
                case eUIGarageStateFilter.All:
                    {
                        LicensesPlatesList = m_GarageSystem.GetLicensePlatesList();
                        break;
                    }
                case eUIGarageStateFilter.InRepair:
                    {
                        LicensesPlatesList = m_GarageSystem.GetLicensePlatesListByGarageState(eVehicleGarageState.InRepair);
                        break;
                    }
                case eUIGarageStateFilter.Repaired:
                    {
                        LicensesPlatesList = m_GarageSystem.GetLicensePlatesListByGarageState(eVehicleGarageState.Repaired);
                        break;
                    }
                case eUIGarageStateFilter.Paid:
                    {
                        LicensesPlatesList = m_GarageSystem.GetLicensePlatesListByGarageState(eVehicleGarageState.Paid);
                        break;
                    }
            }

            return LicensesPlatesList;
        }

        private void changeVehicleGarageState()
        {
            try
            {
                string licensePlate = getLicensePlateFromUser();
                eVehicleGarageState newState = getVehicleGarageStateOptionFromUser();

                m_GarageSystem.ChangeVehicleState(licensePlate, newState);
                Console.WriteLine("Changed vehicle garage state successfully.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Error: {0}, try again.", ex.Message);
            }
        }

        private eVehicleGarageState getVehicleGarageStateOptionFromUser()
        {
            const int k_MinOptionVal = 1;
            const int k_MaxOptionVal = 3;
            bool isValid = false;
            string userInput;
            eVehicleGarageState? userOptionNumber = null;

            m_MessagesToPrint.GarageStateOptionsMessage();
            while (!isValid)
            {
                userInput = Console.ReadLine();
                isValid = Enum.TryParse(userInput, out eVehicleGarageState enumOptionNumber);
                if (isValid)
                {
                    userOptionNumber = enumOptionNumber;
                    break;
                }

                Console.WriteLine("Input must be between {0} to {1}, try again.",
                k_MinOptionVal, k_MaxOptionVal);
            }

            return userOptionNumber.Value;
        }

        private void fillWheelsWithAirToMaximum()
        {
            try
            {
                string licensePlate = getLicensePlateFromUser();

                m_GarageSystem.FillVehicleWheelsWithAir(licensePlate);
                Console.WriteLine("Filled vehicle wheels with maximum air successfully.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Error: {0}, try again.", ex.Message);
            }
        }

        private void chargeFuelVehicle()
        {
            try
            {
                string licensePlate = getLicensePlateFromUser();
                float fuelAmountToAdd = getFuelAmountToAdd();
                string fuelType = getFuelTypeAsString();

                m_GarageSystem.AddFuelToVehicle(licensePlate, fuelAmountToAdd, fuelType);
                Console.WriteLine("Charged fuel vehicle successfully.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Error: {0}, try again.", ex.Message);
            }
            catch (ValueOutOfRangeException ex)
            {
                Console.WriteLine("Error: {0} need to be between {1} to {2}, try again.",
                    ex.Message, ex.MinValue, ex.MaxValue);
                chargeFuelVehicle();
            }
        }

        private float getFuelAmountToAdd()
        {
            Console.Write("Please enter fuel amount to add: ");
            float fuelAmountToAdd = getValidFloatEnergyToAdd();

            return fuelAmountToAdd;
        }

        private float getValidFloatEnergyToAdd()
        {
            bool isValid = false;
            float? fuelAmountToAdd = null;

            while (!isValid)
            {
                string fuelAsString = Console.ReadLine();

                isValid = float.TryParse(fuelAsString, out float tempfuelAmountToAdd);
                if (isValid)
                {
                    fuelAmountToAdd = tempfuelAmountToAdd;
                    break;
                }

                Console.WriteLine("Input must be float number, try again.");
            }

            return fuelAmountToAdd.Value;
        }

        private string getFuelTypeAsString()
        {
            List<string> fuelTypes = m_GarageSystem.GetFuelTypesList();

            Console.WriteLine("Please enter fuel type from the list below:");
            foreach (string fuelType in fuelTypes)
            {
                Console.WriteLine("{0}", fuelType);
            }

            Console.Write("Your choice: ");
            string fuelTypeFromUser = Console.ReadLine();

            return fuelTypeFromUser;
        }

        private void chargeElectricVehicle()
        {
            try
            {
                string licensePlate = getLicensePlateFromUser();
                float electricityMinutesToAdd = getElectricityMinutesToAdd();
                float electricityHoursToAdd = electricityMinutesToAdd / 60f;

                m_GarageSystem.AddElectricityToVehicle(licensePlate, electricityHoursToAdd);
                Console.WriteLine("Charge electric vehicle successfully.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Error: {0}, try again.", ex.Message);
            }
            catch (ValueOutOfRangeException ex)
            {
                Console.WriteLine("Error: {0} need to be between {1} to {2}, try again.",
                    ex.Message, ex.MinValue, ex.MaxValue);
                chargeElectricVehicle();
            }
        }  
        
        private float getElectricityMinutesToAdd()
        {
            Console.WriteLine("Please enter electricity minutes to charge");
            float electricityMinutesAsString = getValidFloatEnergyToAdd();
            
            return electricityMinutesAsString;
        }

        private void displayClientData()
        {
            try
            {
                string licensePlate = getLicensePlateFromUser();
                string clientDataToPrint = m_GarageSystem.GetClientData(licensePlate);

                Console.WriteLine(clientDataToPrint);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Error: {0}, try again.", ex.Message);
            }
        }  
    }
}
