using Fofx.Quintessence.RelationshipSeries.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Fofx
{
    public class RelationshipArrayNumericValueRequestHelper : BaseRelationshipRevisableRequestHelper, ITimeSeriesRequestHelper
    {
        #region Public methods

        public override INullableReader GetDataReader(int[] entites, int[] factors, DatabaseRequestArgs args)
        {
            return default;
        }

        public override INullableReader GetDataReader(int[] entites, int[] factors, int[] relationships, DatabaseRequestArgs args)
        {
            SqlParameter[] parameters = GetParameters(entites, factors, relationships, args);
            return DataAccess.GetDataReader(AppConstants.TimeSeriesLoadConstant, parameters);
        }

        public override ITimeSeries CreateNew(ITimeSeriesIterator iterator, TimeSeriesDescriptor factor)
        {
            return new NumericConstituentArrayTimeSeries();
        }

        public override void Add(ITimeSeries iTimeSeries, INullableReader reader, DatabaseRequestArgs args,
                                 TimeSeriesDatabaseContext requester)
        {
            int toEntityID = reader.GetInt32(2);
            DateTime valueDate = reader.GetDateTime(3);
            DateTime declarationDate = reader.GetDateTime(4);
            double? value = reader.GetNullableDouble(5);
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
         ((NumericConstituentArrayTimeSeries)iTimeSeries).Add(entity, valueDate, declarationDate, value, nonKeyedAttributeSet);
        }

        #endregion

        #region Protected methods

        protected override void Read(int[] entities, int[] timeseries, int[] relationship, DatabaseRequestArgs args,
                                     TimeSeriesDatabaseContext requester, Dictionary<QuickID, ITimeSeriesKey> keys,
                                     SortedList<ITimeSeriesKey, ITimeSeries> dbTimeSeriesResult)
        {
            NumericConstituentArrayTimeSeries ncs = null;
            ITimeSeries referenceTimeSeries = null;
            ITimeSeriesKey referenceKey = null;

            Preference codeTypePreference = null;
            int previousEntityID = -1;
            int previousTimeSeriesValueID = -1;
            int previousRelationshipValueID = -1;
            DateTime previousValueDate = DateTime.MinValue;
            DateTime previousDeclarationDate = DateTime.MinValue;

            SortedList<DateTime, Revision<ConstituentNumericArrayPoint>> pointCollection = null;
            Revision<ConstituentNumericArrayPoint> revisionCollection = null;
            ConstituentNumericArrayPoint constituentPoint = null;

            using (INullableReader reader = GetDataReader(entities, timeseries, relationship, args))
            {
                while (reader.Read())
                {
                    int entityID = reader.GetInt32(0);
                    int timeSeriesValueID = reader.GetInt32(1);
                    int relationshipValueID = reader.GetInt32(2);
                    int toEntityID = reader.GetInt32(3);
                    DateTime valueDate = reader.GetDateTime(4);
                    DateTime declarationDate = reader.GetDateTime(5);
                    double? value = reader.GetNullableDouble(6);
                    int? nonKeyedAttributeSetId = reader.GetNullableInt32(7);
                    IEntityDescriptor entity = null;

                    NonKeyedAttributeSet nonKeyedAttributeSet = null;
                    if (nonKeyedAttributeSetId != null)
                        nonKeyedAttributeSet = args.Translator.GetNonKeyedAttributeSet((int)nonKeyedAttributeSetId);

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
                                ncs = referenceTimeSeries as NumericConstituentArrayTimeSeries;
                                if (ncs != null)
                                {
                                    ncs.Add(pointCollection);
                                }
                            }
                        }

                        pointCollection = new SortedList<DateTime, Revision<ConstituentNumericArrayPoint>>();
                        revisionCollection = new Revision<ConstituentNumericArrayPoint>(valueDate);
                        pointCollection.Add(valueDate, revisionCollection);
                        constituentPoint = new ConstituentNumericArrayPoint(declarationDate, valueDate, nonKeyedAttributeSet);
                        revisionCollection.Add(declarationDate, constituentPoint);
                        previousEntityID = entityID;
                        previousTimeSeriesValueID = timeSeriesValueID;
                        previousRelationshipValueID = relationshipValueID;
                        previousValueDate = valueDate;
                        previousDeclarationDate = declarationDate;
                    }
                    else if (valueDate != previousValueDate)
                    {
                        revisionCollection = new Revision<ConstituentNumericArrayPoint>(valueDate);
                        pointCollection.Add(valueDate, revisionCollection);
                        constituentPoint = new ConstituentNumericArrayPoint(declarationDate, valueDate, nonKeyedAttributeSet);
                        revisionCollection.Add(declarationDate, constituentPoint);
                        previousValueDate = valueDate;
                        previousDeclarationDate = declarationDate;
                    }
                    else if (declarationDate != previousDeclarationDate)
                    {
                        constituentPoint = new ConstituentNumericArrayPoint(declarationDate, valueDate, nonKeyedAttributeSet);
                        revisionCollection.Add(declarationDate, constituentPoint);
                        previousValueDate = valueDate;
                        previousDeclarationDate = declarationDate;
                    }

                    if (!requester.EntityLookup.TryGetValue(toEntityID, out entity))
                    {
                        if (!args.Translator.TryGetEntityDescriptorByID(toEntityID, codeTypePreference, out entity))
                            entity = new EntityDescriptor(toEntityID);
                        else
                            requester.EntityLookup.Add(toEntityID, entity);
                    }

                    constituentPoint.Add(entity, new double[] { value.Value }, nonKeyedAttributeSet);
                }
            }

            QuickID qid2 = new QuickID(previousEntityID, previousTimeSeriesValueID, previousRelationshipValueID);
            if (keys.TryGetValue(qid2, out referenceKey))
            {
                if (dbTimeSeriesResult.TryGetValue(referenceKey, out referenceTimeSeries))
                {
                    ncs = referenceTimeSeries as NumericConstituentArrayTimeSeries;
                    if (ncs != null)
                    {
                        ncs.Add(pointCollection);
                    }
                }
            }
        }

        #endregion
    }
}
