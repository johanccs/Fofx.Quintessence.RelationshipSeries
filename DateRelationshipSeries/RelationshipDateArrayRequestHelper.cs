using Fofx.Quintessence.RelationshipSeries.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Fofx
{
    public class RelationshipArrayDateValueRequestHelper : BaseRelationshipRevisableRequestHelper, ITimeSeriesRequestHelper
    {
        #region Public methods

        public override INullableReader GetDataReader(int[] entites, int[] factors, DatabaseRequestArgs args)
        {
            throw new NotImplementedException("");
        }

        public override INullableReader GetDataReader(int[] entites, int[] factors, int[] relationships, DatabaseRequestArgs args)
        {
            SqlParameter[] parameters = GetParameters(entites, factors, relationships, args);
            return DataAccess.GetDataReader(AppConstants.TimeSeriesLoadConstant, parameters);
        }

        public override ITimeSeries CreateNew(ITimeSeriesIterator iterator, TimeSeriesDescriptor factor)
        {
            return new DateConstituentArrayTimeSeries();
        }

        public override void Add(ITimeSeries iTimeSeries, INullableReader reader, DatabaseRequestArgs args, TimeSeriesDatabaseContext requester)
        {
            int toEntityID = reader.GetInt32(2);
            DateTime valueDate = reader.GetDateTime(3);
            DateTime declarationDate = reader.GetDateTime(4);
            string value = reader.GetString(5);
            int? nonKeyedAttributeSetId = reader.GetNullableInt32(7);

            NonKeyedAttributeSet nonKeyedAttributeSet = null;
            if (nonKeyedAttributeSetId != null)
                nonKeyedAttributeSet = args.Translator.GetNonKeyedAttributeSet((int)nonKeyedAttributeSetId);


            IEntityDescriptor entity = null;
            if (!requester.EntityLookup.TryGetValue(toEntityID, out entity))
            {
                if (!args.Translator.TryGetEntityDescriptorByID(toEntityID, out entity))
                    entity = new EntityDescriptor(toEntityID);
            }
          ((DateConstituentArrayTimeSeries)iTimeSeries).Add(entity, valueDate, declarationDate, value, nonKeyedAttributeSet);
        }

        #endregion

        #region Protected Methods

        protected override void Read(int[] entities,
                                     int[] timeseries,
                                     int[] relationship,
                                     DatabaseRequestArgs args,
                                     TimeSeriesDatabaseContext requester,
                                     Dictionary<QuickID, ITimeSeriesKey> keys,
                                     SortedList<ITimeSeriesKey, ITimeSeries> dbTimeSeriesResult)
        {
            DateConstituentArrayTimeSeries ncs = null;
            ITimeSeries referenceTimeSeries = null;
            ITimeSeriesKey referenceKey = null;

            Preference codeTypePreference = null;
            int previousEntityID = -1;
            int previousTimeSeriesValueID = -1;
            int previousRelationshipValueID = -1;
            DateTime previousValueDate = DateTime.MinValue;
            DateTime previousDeclarationDate = DateTime.MinValue;

            SortedList<DateTime, Revision<ConstituentDateArrayPoint>> pointCollection = null;
            Revision<ConstituentDateArrayPoint> revisionCollection = null;
            ConstituentDateArrayPoint constituentPoint = null;

            using (INullableReader reader = GetDataReader(entities, timeseries, relationship, args))
            {
                Read(args, requester, keys, dbTimeSeriesResult, ref ncs, ref referenceTimeSeries, ref referenceKey,
                     ref codeTypePreference, ref previousEntityID, ref previousTimeSeriesValueID,
                     ref previousRelationshipValueID, ref previousValueDate, ref previousDeclarationDate,
                     ref pointCollection, ref revisionCollection, ref constituentPoint, reader);
            }

            QuickID qid2 = new QuickID(previousEntityID, previousTimeSeriesValueID, previousRelationshipValueID);
            if (keys.TryGetValue(qid2, out referenceKey))
            {
                if (dbTimeSeriesResult.TryGetValue(referenceKey, out referenceTimeSeries))
                {
                    ncs = referenceTimeSeries as DateConstituentArrayTimeSeries;
                    if (ncs != null)
                    {
                        ncs.Add(pointCollection);
                    }
                }
            }
        }

        #endregion

        #region Private methods
        private void Read(DatabaseRequestArgs args, TimeSeriesDatabaseContext requester,
                          Dictionary<QuickID, ITimeSeriesKey> keys,
                          SortedList<ITimeSeriesKey, ITimeSeries> dbTimeSeriesResult,
                          ref DateConstituentArrayTimeSeries ncs, ref ITimeSeries referenceTimeSeries,
                          ref ITimeSeriesKey referenceKey, ref Preference codeTypePreference, ref int previousEntityID,
                          ref int previousTimeSeriesValueID, ref int previousRelationshipValueID,
                          ref DateTime previousValueDate, ref DateTime previousDeclarationDate,
                          ref SortedList<DateTime, Revision<ConstituentDateArrayPoint>> pointCollection,
                          ref Revision<ConstituentDateArrayPoint> revisionCollection,
                          ref ConstituentDateArrayPoint constituentPoint, INullableReader reader)
        {
            while (reader.Read())
            {
                int entityID = reader.GetInt32(0);
                int timeSeriesValueID = reader.GetInt32(1);
                int relationshipValueID = reader.GetInt32(2);
                int toEntityID = reader.GetInt32(3);
                DateTime valueDate = reader.GetDateTime(4);
                DateTime declarationDate = reader.GetDateTime(5);
                string value = reader.GetNullableString(6);
                int? nonKeyedAttributeSetId = reader.GetNullableInt32(7);

                NonKeyedAttributeSet nonKeyedAttributeSet = null;
                if (nonKeyedAttributeSetId != null)
                    nonKeyedAttributeSet = args.Translator.GetNonKeyedAttributeSet((int)nonKeyedAttributeSetId);

                IEntityDescriptor entity = null;

                DoValidation(requester, keys, dbTimeSeriesResult, ref ncs, ref referenceTimeSeries, ref referenceKey,
                    ref codeTypePreference, ref previousEntityID, ref previousTimeSeriesValueID,
                    ref previousRelationshipValueID, ref previousValueDate, ref previousDeclarationDate,
                    ref pointCollection, ref revisionCollection, ref constituentPoint, entityID, timeSeriesValueID,
                    relationshipValueID, valueDate, declarationDate, nonKeyedAttributeSet);

                if (!requester.EntityLookup.TryGetValue(toEntityID, out entity))
                {
                    if (!args.Translator.TryGetEntityDescriptorByID(toEntityID, codeTypePreference, out entity))
                        entity = new EntityDescriptor(toEntityID);
                }

                constituentPoint.Add(entity, value, nonKeyedAttributeSet);
            }
        }

        private void DoValidation(TimeSeriesDatabaseContext requester, Dictionary<QuickID, ITimeSeriesKey> keys,
                                         SortedList<ITimeSeriesKey, ITimeSeries> dbTimeSeriesResult,
                                         ref DateConstituentArrayTimeSeries ncs, ref ITimeSeries referenceTimeSeries,
                                         ref ITimeSeriesKey referenceKey, ref Preference codeTypePreference,
                                         ref int previousEntityID, ref int previousTimeSeriesValueID,
                                         ref int previousRelationshipValueID, ref DateTime previousValueDate,
                                         ref DateTime previousDeclarationDate,
                                         ref SortedList<DateTime, Revision<ConstituentDateArrayPoint>> pointCollection,
                                         ref Revision<ConstituentDateArrayPoint> revisionCollection,
                                         ref ConstituentDateArrayPoint constituentPoint, int entityID,
                                         int timeSeriesValueID, int relationshipValueID, DateTime valueDate,
                                         DateTime declarationDate, NonKeyedAttributeSet nonKeyedAttributeSet)
        {
            if (entityID != previousEntityID || previousTimeSeriesValueID != timeSeriesValueID || previousRelationshipValueID != relationshipValueID)
            {
                QuickID qid = new QuickID(entityID, timeSeriesValueID, relationshipValueID);
                if (keys.TryGetValue(qid, out referenceKey))
                {
                    RelationshipTimeSeriesKey tsk = referenceKey as RelationshipTimeSeriesKey;
                    if (tsk == null || tsk.Relationship == null || tsk.Relationship.Source == null)
                        codeTypePreference = null;
                    else
                    {
                        if (codeTypePreference != null && tsk.Relationship.Source.CodePreference != null)
                        {
                            if (codeTypePreference.Id != tsk.Relationship.Source.CodePreference.Id)
                                requester.EntityLookup.Clear();

                            codeTypePreference = tsk.Relationship.Source.CodePreference;
                        }
                        else
                        {
                            requester.EntityLookup.Clear();
                            codeTypePreference = tsk.Relationship.Source.CodePreference;
                        }
                    }
                }

                qid = new QuickID(previousEntityID, previousTimeSeriesValueID, previousRelationshipValueID);
                if (keys.TryGetValue(qid, out referenceKey))
                {
                    if (dbTimeSeriesResult.TryGetValue(referenceKey, out referenceTimeSeries))
                    {
                        ncs = referenceTimeSeries as DateConstituentArrayTimeSeries;
                        if (ncs != null)
                        {
                            ncs.Add(pointCollection);
                        }
                    }
                }

                AssignVariableValues(out previousEntityID, out previousTimeSeriesValueID,
                                     out previousRelationshipValueID, out previousValueDate, out previousDeclarationDate,
                                     out pointCollection, out revisionCollection, out constituentPoint, entityID,
                                     timeSeriesValueID, relationshipValueID, valueDate, declarationDate,
                                     nonKeyedAttributeSet);
            }
            else if (valueDate != previousValueDate)
            {
                ValidateValueDate(out previousValueDate, out previousDeclarationDate, pointCollection,
                                  out revisionCollection, out constituentPoint, valueDate, declarationDate,
                                  nonKeyedAttributeSet);
            }
            else if (declarationDate != previousDeclarationDate)
            {
                ValidateDeclarationDate(out previousValueDate, out previousDeclarationDate, revisionCollection,
                                        out constituentPoint, valueDate, declarationDate, nonKeyedAttributeSet);
            }
        }

        private void AssignVariableValues(out int previousEntityID, out int previousTimeSeriesValueID,
                                                 out int previousRelationshipValueID, out DateTime previousValueDate,
                                                 out DateTime previousDeclarationDate,
                                                 out SortedList<DateTime, Revision<ConstituentDateArrayPoint>> pointCollection,
                                                 out Revision<ConstituentDateArrayPoint> revisionCollection,
                                                 out ConstituentDateArrayPoint constituentPoint, int entityID,
                                                 int timeSeriesValueID, int relationshipValueID, DateTime valueDate,
                                                 DateTime declarationDate, NonKeyedAttributeSet nonKeyedAttributeSet)
        {
            pointCollection = new SortedList<DateTime, Revision<ConstituentDateArrayPoint>>();
            revisionCollection = new Revision<ConstituentDateArrayPoint>(valueDate);
            pointCollection.Add(valueDate, revisionCollection);
            constituentPoint = new ConstituentDateArrayPoint(declarationDate, valueDate, nonKeyedAttributeSet);
            revisionCollection.Add(declarationDate, constituentPoint);
            previousEntityID = entityID;
            previousTimeSeriesValueID = timeSeriesValueID;
            previousRelationshipValueID = relationshipValueID;
            previousValueDate = valueDate;
            previousDeclarationDate = declarationDate;
        }

        private static void ValidateDeclarationDate(out DateTime previousValueDate, out DateTime previousDeclarationDate, Revision<ConstituentDateArrayPoint> revisionCollection, out ConstituentDateArrayPoint constituentPoint, DateTime valueDate, DateTime declarationDate, NonKeyedAttributeSet nonKeyedAttributeSet)
        {
            constituentPoint = new ConstituentDateArrayPoint(declarationDate, valueDate, nonKeyedAttributeSet);
            revisionCollection.Add(declarationDate, constituentPoint);
            previousValueDate = valueDate;
            previousDeclarationDate = declarationDate;
        }

        private static void ValidateValueDate(out DateTime previousValueDate, out DateTime previousDeclarationDate, SortedList<DateTime, Revision<ConstituentDateArrayPoint>> pointCollection, out Revision<ConstituentDateArrayPoint> revisionCollection, out ConstituentDateArrayPoint constituentPoint, DateTime valueDate, DateTime declarationDate, NonKeyedAttributeSet nonKeyedAttributeSet)
        {
            revisionCollection = new Revision<ConstituentDateArrayPoint>(valueDate);
            pointCollection.Add(valueDate, revisionCollection);
            constituentPoint = new ConstituentDateArrayPoint(declarationDate, valueDate, nonKeyedAttributeSet);
            revisionCollection.Add(declarationDate, constituentPoint);
            previousValueDate = valueDate;
            previousDeclarationDate = declarationDate;
        }

        #endregion
    }
}
