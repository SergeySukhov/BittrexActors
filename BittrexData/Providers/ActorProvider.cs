﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BittrexData.Contexts;
using BittrexData.Interfaces;
using BittrexData.Models; // !!
using Microsoft.EntityFrameworkCore;

namespace BittrexData.Providers
{
    public class ActorProvider : IActorProvider
    {
		
		public async Task SaveOrUpdateActor(ActorDataDto actorData)
		{
			var context = new BittrexActorsDbContext();

			try
			{
				var savedData = context.ActorDatas.Where(actor => actor.Guid == actorData.Guid)
                    .Include(x => x.Rules)
                    .Include(x => x.Transactions)
                    .Include(x => x.Account)
                    .Include(x => x.Predictions)
                    .FirstOrDefault();

				if (savedData == null)
				{
					context.ActorDatas.Add(actorData);
				} else
				{
					context.ActorDatas.Remove(savedData);
					context.ActorDatas.Add(actorData);
				}
                await context.SaveChangesAsync();
                await context.DisposeAsync();
				return;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Ошибка!", ex.Message);
				if (context != null) context.Dispose();
				return;
			}

		} 

		public async Task<List<ActorDataDto>> LoadAliveActors()
		{
			var context = new BittrexActorsDbContext();

			var aliveActors = context.ActorDatas.Where(actor => actor.IsAlive)
                .Include(x => x.Rules)
                .Include(x => x.Account)
                .Include(x => x.Transactions)
                .Include(x => x.Predictions)
                .ToList();

			await context.DisposeAsync();

			return aliveActors;
		}

        public async Task ClearOldData()
        {
            var context = new BittrexActorsDbContext();
            var actors = context.ActorDatas.Where(actor => actor.Guid.ToString() != "")
            .ToList();

            if (actors != null) 
            context.ActorDatas.RemoveRange(actors);

            await context.SaveChangesAsync();
            await context.DisposeAsync();
        }
	}
}
