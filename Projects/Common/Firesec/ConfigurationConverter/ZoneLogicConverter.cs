﻿using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Firesec.Models.ZonesLogic;
using FiresecAPI;
using FiresecAPI.Models;

namespace Firesec
{
	public static class ZoneLogicConverter
	{
		public static ZoneLogic Convert(DeviceConfiguration deviceConfiguration, expr innerZoneLogic)
		{
			var zoneLogic = new ZoneLogic();

			if (innerZoneLogic != null && innerZoneLogic.clause.IsNotNullOrEmpty())
			{
				foreach (var innerClause in innerZoneLogic.clause)
				{
					var clause = new Clause();
					if (innerClause.zone != null)
					{
						foreach (var item in innerClause.zone)
						{
                            if (string.IsNullOrWhiteSpace(item) == false)
                            {
                                var zoneNo = int.Parse(item);
                                var zone = deviceConfiguration.Zones.FirstOrDefault(x => x.No == zoneNo);
                                if (zone != null)
                                {
                                    clause.ZoneUIDs.Add(zone.UID);
                                }
                            }
						}
					}
					if (innerClause.device != null)
					{
						var innerDevice = innerClause.device.FirstOrDefault();
						if (innerDevice != null)
						{
							if (!string.IsNullOrEmpty(innerDevice.UID))
							{
								clause.DeviceUID = GuidHelper.ToGuid(innerDevice.UID);
							}
						}
					}

					clause.State = (ZoneLogicState)int.Parse(innerClause.state);

					switch (innerClause.operation)
					{
						case "and":
							clause.Operation = ZoneLogicOperation.All;
							break;

						case "or":
							clause.Operation = ZoneLogicOperation.Any;
							break;

						default:
							clause.Operation = null;
							break;
					}

					switch (innerClause.joinOperator)
					{
						case "and":
							zoneLogic.JoinOperator = ZoneLogicJoinOperator.And;
							break;

						case "or":
							zoneLogic.JoinOperator = ZoneLogicJoinOperator.Or;
							break;
					}

					zoneLogic.Clauses.Add(clause);
				}
			}

			return zoneLogic;
		}

		public static expr ConvertBack(ZoneLogic zoneLogic)
		{
			var innerZoneLogic = new expr();

			var innerClauses = new List<clauseType>();
			foreach (var clause in zoneLogic.Clauses)
			{
				var innerClause = new clauseType();
				innerClause.state = ((int)clause.State).ToString();

				switch (clause.Operation)
				{
					case ZoneLogicOperation.All:
						innerClause.operation = "and";
						break;

					case ZoneLogicOperation.Any:
						innerClause.operation = "or";
						break;

					default:
						innerClause.operation = null;
						break;
				}

				switch (zoneLogic.JoinOperator)
				{
					case ZoneLogicJoinOperator.And:
						innerClause.joinOperator = "and";
						break;

					case ZoneLogicJoinOperator.Or:
						innerClause.joinOperator = "or";
						break;

					default:
						innerClause.joinOperator = null;
						break;
				}

				if (clause.DeviceUID != Guid.Empty)
				{
					innerClause.device = new deviceType[1];
					innerClause.device[0] = new deviceType() { UID = clause.DeviceUID.ToString() };
				}

                innerClause.zone = clause.Zones.Select(x => x.No.ToString()).ToArray();
				innerClauses.Add(innerClause);
			}

			innerZoneLogic.clause = innerClauses.ToArray();
			return innerZoneLogic;
		}
	}
}