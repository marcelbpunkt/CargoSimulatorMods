using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MakeItWork
{
    internal static class RebalanceQueuePatch
    {
        internal static void DeskQueueController_DequeueCustomer_Post(ref DeskQueueController __instance)
        {
            List<DeskQueueController> cashRegisters = __instance._deskGroupController._allDesk;
            cashRegisters.RemoveAll((DeskQueueController desk) => desk is null || !desk.IsEnabled || !desk.IsDeskOpen);
            if (cashRegisters.Count < 2)
                return;

            cashRegisters = cashRegisters.OrderBy(desk => desk.CustomerInQueue.Count).ToList();
            DeskQueueController shortest = cashRegisters.First();
            DeskQueueController longest = cashRegisters.Last();
            while (longest.CustomerInQueue.Count - shortest.CustomerInQueue.Count >= 2)
            {
                // move last customer from the longest to the shortest queue
                CustomerController customer = longest.CustomerInQueue.Last();
                MoveCustomerFromLongestToShortestQueue(ref customer, ref longest, ref shortest);


                // re-sort queues, rinse and repeat
                cashRegisters = cashRegisters.OrderBy(cashRegister => cashRegister.CustomerInQueue.Count).ToList();
                shortest = cashRegisters.First();
                longest = cashRegisters.Last();
            }
        }

        private static void MoveCustomerFromLongestToShortestQueue(ref CustomerController customer,
            ref DeskQueueController longest, ref DeskQueueController shortest)
        {
            // manual partial rewriting of DeskQueueController.DequeueCustomer() so the "next please" part is not
            // executed
            ((Component)customer).transform.parent = null;
            longest.CustomerInQueue.Remove(customer);
            longest._currentQueueCount = Mathf.Clamp(0, longest._currentQueueCount - 1, longest._maxQueueCount);
            longest.CheckAvaliable();

            customer.Status = CustomerController.CustomerStatus.Spawned;
            customer.DirectToDesk();
        }
    }
}
