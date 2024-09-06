using System.Collections;
using CattleyaDecrypto.Server.Services.Interfaces;

namespace CattleyaDecrypto.Server.Services;

public class OrderGeneratorService : IOrderGeneratorService
{
    public int[] GetRandomOrder(ref int number)
    {
        var bitArray = new BitArray(BitConverter.GetBytes(number));
        var availableOrders = bitArray
            .Cast<bool>()
            .Select((val, index) => (val, index))
            .Where(v => v.val)
            .Select(v => v.index)
            .ToArray();
    
        var order = availableOrders[Random.Shared.Next(0, availableOrders.Length - 1)];
    
        bitArray[order] = false;
        var array = new int[1];
        bitArray.CopyTo(array, 0);
        number = array[0];

        int firstNumber = order / 6;
        var numberOrder = new List<int> { 0, 1, 2, 3 };
        numberOrder.Remove(firstNumber);
        var secondNumber = numberOrder[order % 6 / 2];
        numberOrder.Remove(secondNumber);
        var thirdNumber = numberOrder[order % 2];

        return [firstNumber, secondNumber, thirdNumber];
    }
}