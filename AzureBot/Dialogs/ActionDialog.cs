﻿namespace AzureBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis;

    [LuisModel("c9e598cb-0e5f-48f6-b14a-ebbb390a6fb3", "a7c1c493d0e244e796b83c6785c4be4d")]
    [Serializable]
    public class ActionDialog : LuisDialog<string>
    {
        private static string[] ordinals = { "first", "second", "third", "fourth", "fifth" };

        private readonly string originalMessage;

        public ActionDialog(string originalMessage)
        {
            this.originalMessage = originalMessage;
        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry I did not understand: " + string.Join(", ", result.Intents.Select(i => i.Intent));
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("ListSubscriptions")]
        public async Task ListSubscriptionsAsync(IDialogContext context, LuisResult result)
        {
            int index = 0;
            var subscriptions = GetAllSubscriptions().Aggregate(string.Empty, (current, next) =>
            {
                index++;
                return current += $"\r\n{index}. {next}";
            });

            await context.PostAsync($"Your subscriptions are: {subscriptions}");
            context.Wait(MessageReceived);
        }

        [LuisIntent("UseSubscription")]
        public async Task UseSubscriptionAsync(IDialogContext context, LuisResult result)
        {
            var entity = result.Entities.OrderByDescending(p => p.Score).FirstOrDefault();
            if (entity != null)
            {
                var subscriptionName = entity.Entity;
                if (entity.Type == "builtin.ordinal")
                {
                    var ordinal = Array.IndexOf(ordinals, entity.Entity.ToLowerInvariant());
                    if (ordinal >= 0)
                    {
                        subscriptionName = GetAllSubscriptions().ElementAt(ordinal);
                    }
                }
                await context.PostAsync($"Using the {subscriptionName} subscription.");
            }
            else
            {
                await context.PostAsync("Which subscription do you want to use?");
            }

            context.Wait(MessageReceived);
        }

        [LuisIntent("ListVms")]
        public async Task ListVmsAsync(IDialogContext context, LuisResult result)
        {
            int index = 0;
            var virtualMachines = GetAllVms().Aggregate(string.Empty, (current, next) =>
            {
                index++;
                return current += $"\r\n{index}. {next}";
            });

            await context.PostAsync($"Available VMs are: {virtualMachines}");
            context.Wait(MessageReceived);
        }

        [LuisIntent("StartVm")]
        public async Task StartVmAsync(IDialogContext context, LuisResult result)
        {
            var entity = result.Entities.OrderByDescending(p => p.Score).FirstOrDefault();
            if (entity != null)
            {
                var virtualMachineName = entity.Entity;
                if (entity.Type == "builtin.ordinal")
                {
                    var ordinal = Array.IndexOf(ordinals, entity.Entity.ToLowerInvariant());
                    if (ordinal >= 0)
                    {
                        virtualMachineName = GetAllVms().ElementAt(ordinal);
                    }
                }

                await context.PostAsync($"Starting the {virtualMachineName} virtual machine.");
            }
            else
            {
                await context.PostAsync("Which virtual machine do you want to start?");
            }

            context.Wait(MessageReceived);
        }

        [LuisIntent("StopVm")]
        public async Task StopVmAsync(IDialogContext context, LuisResult result)
        {
            var entity = result.Entities.OrderByDescending(p => p.Score).FirstOrDefault();
            if (entity != null)
            {
                var virtualMachineName = entity.Entity;
                if (entity.Type == "builtin.ordinal")
                {
                    var ordinal = Array.IndexOf(ordinals, entity.Entity.ToLowerInvariant());
                    if (ordinal >= 0)
                    {
                        virtualMachineName = GetAllVms().ElementAt(ordinal);
                    }
                }

                await context.PostAsync($"Stopping the {virtualMachineName} virtual machine.");
            }
            else
            {
                await context.PostAsync("Which virtual machine do you want to stop?");
            }

            context.Wait(MessageReceived);
        }

        [LuisIntent("RunRunbook")]
        public async Task RunRunbookAsync(IDialogContext context, LuisResult result)
        {
            var entity = result.Entities.OrderByDescending(p => p.Score).FirstOrDefault();
            if (entity != null)
            {
                var runbookName = entity.Entity;
                await context.PostAsync($"Launching the {runbookName} runbook.");
            }
            else
            {
                await context.PostAsync("Which runbook do you want to run?");
            }

            context.Wait(MessageReceived);
        }

        // TODO - move Azure operations to a separate class
        private IEnumerable<string> GetAllSubscriptions()
        {
            return new string[] { "Development", "Staging", "Production", "QA" };
        }

        private IEnumerable<string> GetAllVms()
        {
            return new string[] { "svrcomm01", "svrcomm02", "svrapipub", "svrdbprod" };
        }
    }
}