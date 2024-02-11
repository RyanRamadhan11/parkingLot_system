using System;
using System.Collections.Generic;
using System.Linq;

public enum VehicleType
{
    Car,
    Motorcycle
}

public enum ParkingStatus
{
    Available,
    Occupied
}

public class ParkingSlot
{
    public int SlotNumber { get; set; }
    public ParkingStatus Status { get; set; }
    public VehicleType Type { get; set; }
    public string RegistrationNumber { get; set; }
    public string Color { get; set; }
}

public class ParkingLot
{
    private List<ParkingSlot> slots;

    public ParkingLot(int capacity)
    {
        slots = new List<ParkingSlot>(capacity);
        for (int i = 0; i < capacity; i++)
        {
            slots.Add(new ParkingSlot
            {
                SlotNumber = i + 1,
                Status = ParkingStatus.Available
            });
        }
    }

    public int TotalSlots => slots.Count;
    public int OccupiedSlots => slots.Count(slot => slot.Status == ParkingStatus.Occupied);
    public int AvailableSlots => slots.Count(slot => slot.Status == ParkingStatus.Available);

    public void Park(string registrationNumber, string color, VehicleType type)
    {
        ParkingSlot availableSlot = slots.FirstOrDefault(slot => slot.Status == ParkingStatus.Available);
        if (availableSlot == null)
        {
            Console.WriteLine("Sorry, parking lot is full");
            return;
        }

        availableSlot.Status = ParkingStatus.Occupied;
        availableSlot.RegistrationNumber = registrationNumber;
        availableSlot.Color = color;
        availableSlot.Type = type;

        Console.WriteLine($"Allocated slot number: {availableSlot.SlotNumber}");
    }

    public void Leave(int slotNumber)
    {
        ParkingSlot slotToLeave = slots.FirstOrDefault(slot => slot.SlotNumber == slotNumber);
        if (slotToLeave == null || slotToLeave.Status == ParkingStatus.Available)
        {
            Console.WriteLine($"Slot number {slotNumber} is already empty");
            return;
        }

        slotToLeave.Status = ParkingStatus.Available;
        slotToLeave.RegistrationNumber = null;
        slotToLeave.Color = null;
        slotToLeave.Type = VehicleType.Car; // Reset type to default

        Console.WriteLine($"Slot number {slotNumber} is free");
    }

    public void Status()
    {
        Console.WriteLine("Slot\tNo.\tType\tRegistration No\tColour");
        foreach (var slot in slots)
        {
            Console.WriteLine($"{slot.SlotNumber}\t{slot.Type}\t{slot.RegistrationNumber ?? "-"}\t{slot.Color ?? "-"}");
        }
    }

    public IEnumerable<string> GetRegistrationNumbersForVehiclesWithColor(string color)
    {
        return slots.Where(slot => slot.Color == color && slot.Status == ParkingStatus.Occupied)
                    .Select(slot => slot.RegistrationNumber);
    }

    public IEnumerable<int> GetSlotNumbersForVehiclesWithColor(string color)
    {
        return slots.Where(slot => slot.Color == color && slot.Status == ParkingStatus.Occupied)
                    .Select(slot => slot.SlotNumber);
    }

    public IEnumerable<string> GetRegistrationNumbersForVehiclesWithOddPlate()
    {
        return slots.Where(slot => IsOddPlate(slot.RegistrationNumber) && slot.Status == ParkingStatus.Occupied)
                    .Select(slot => slot.RegistrationNumber);
    }

    public IEnumerable<string> GetRegistrationNumbersForVehiclesWithEvenPlate()
    {
        return slots.Where(slot => !IsOddPlate(slot.RegistrationNumber) && slot.Status == ParkingStatus.Occupied)
                    .Select(slot => slot.RegistrationNumber);
    }

    public IEnumerable<VehicleType> GetTypeOfVehicles()
    {
        return slots.Where(slot => slot.Status == ParkingStatus.Occupied)
                    .Select(slot => slot.Type);
    }

    private bool IsOddPlate(string registrationNumber)
    {
        if (string.IsNullOrEmpty(registrationNumber))
            return false;

        char lastChar = registrationNumber.ToUpper()[registrationNumber.Length - 1];
        return lastChar >= '1' && lastChar <= '9' && (lastChar - '0') % 2 != 0;
    }
}

class Program
{
    static void Main(string[] args)
    {
        ParkingLot parkingLot = null;

        while (true)
        {
            string input = Console.ReadLine();
            string[] tokens = input.Split(' ');

            string command = tokens[0];

            if (command == "create_parking_lot")
            {
                int capacity = int.Parse(tokens[1]);
                parkingLot = new ParkingLot(capacity);
                Console.WriteLine($"Created a parking lot with {capacity} slots");
            }
            else if (command == "park")
            {
                if (parkingLot == null)
                {
                    Console.WriteLine("Parking lot is not created yet");
                    continue;
                }

                string registrationNumber = tokens[1];
                string color = tokens[2];
                VehicleType type = tokens[3] == "Motor" ? VehicleType.Motorcycle : VehicleType.Car;
                parkingLot.Park(registrationNumber, color, type);
            }
            else if (command == "leave")
            {
                if (parkingLot == null)
                {
                    Console.WriteLine("Parking lot is not created yet");
                    continue;
                }

                int slotNumber = int.Parse(tokens[1]);
                parkingLot.Leave(slotNumber);
            }
            else if (command == "status")
            {
                if (parkingLot == null)
                {
                    Console.WriteLine("Parking lot is not created yet");
                    continue;
                }

                parkingLot.Status();
            }
            else if (command == "registration_numbers_for_vehicles_with_color")
            {
                if (parkingLot == null)
                {
                    Console.WriteLine("Parking lot is not created yet");
                    continue;
                }

                string color = tokens[1];
                var registrationNumbers = parkingLot.GetRegistrationNumbersForVehiclesWithColor(color);
                Console.WriteLine(string.Join(", ", registrationNumbers));
            }
            else if (command == "slot_numbers_for_vehicles_with_color")
            {
                if (parkingLot == null)
                {
                    Console.WriteLine("Parking lot is not created yet");
                    continue;
                }

                string color = tokens[1];
                var slotNumbers = parkingLot.GetSlotNumbersForVehiclesWithColor(color);
                Console.WriteLine(string.Join(", ", slotNumbers));
            }
            else if (command == "registration_numbers_for_vehicles_with_odd_plate")
            {
                if (parkingLot == null)
                {
                    Console.WriteLine("Parking lot is not created yet");
                    continue;
                }

                var registrationNumbers = parkingLot.GetRegistrationNumbersForVehiclesWithOddPlate();
                Console.WriteLine(string.Join(", ", registrationNumbers));
            }
            else if (command == "registration_numbers_for_vehicles_with_even_plate")
            {
                if (parkingLot == null)
                {
                    Console.WriteLine("Parking lot is not created yet");
                    continue;
                }

                var registrationNumbers = parkingLot.GetRegistrationNumbersForVehiclesWithEvenPlate();
                Console.WriteLine(string.Join(", ", registrationNumbers));
            }
            else if (command == "type_of_vehicles")
            {
                if (parkingLot == null)
                {
                    Console.WriteLine("Parking lot is not created yet");
                    continue;
                }

                var vehicleTypes = parkingLot.GetTypeOfVehicles();
                Console.WriteLine(vehicleTypes.Count());
            }
            else if (command == "exit")
            {
                break;
            }
            else
            {
                Console.WriteLine("Invalid command");
            }
        }
    }
}
