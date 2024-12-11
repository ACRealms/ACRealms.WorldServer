using ACRealms.Rulesets.Enums;

namespace ACRealms.Rulesets.Loader
{
    public static class Loader
    {
        public static RealmToImport DeserializeRealmJson(string filename, string fileContent)
        {
            var dobj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(fileContent);
            DBOld.Realm realm = new DBOld.Realm();
            realm.Name = dobj.name.Value;
            realm.Type = (ushort)Enum.Parse(typeof(RealmType), dobj.type.Value);
            realm.PropertyCountRandomized = (ushort?)dobj.properties_random_count?.Value;

            if (dobj.parent != null)
                realm.ParentRealmName = dobj.parent.Value;

            if (dobj.properties != null)
            {
                foreach (var prop in ((Newtonsoft.Json.Linq.JObject)dobj.properties).Properties())
                {
                    if (prop.Value.Type == Newtonsoft.Json.Linq.JTokenType.Object)
                    {
                        var pobj = prop.Value.ToObject<RealmPropertyJsonModel>();
                        realm.SetPropertyByName_Complex(prop.Name, pobj);
                    }
                    else
                    {
                        realm.SetPropertyByName(prop.Name, prop.Value);
                    }
                }
            }

            var links = new List<DBOld.RealmRulesetLinks>();
            ushort order = 0;
            if (dobj.apply_rulesets is Newtonsoft.Json.Linq.JArray apply_rulesets)
            {
                foreach (var apply_ruleset in apply_rulesets)
                {
                    var name = (string)apply_ruleset;
                    var link = new DBOld.RealmRulesetLinks();
                    link.Import_RulesetToApply = name;
                    link.Realm = realm;
                    link.Order = ++order;
                    link.LinkType = (ushort)RealmRulesetLinkType.apply_after_inherit;
                    links.Add(link);
                }
            }

            if (dobj.apply_rulesets_random is Newtonsoft.Json.Linq.JArray apply_rulesets_random)
            {
                byte probabilitygroup = 0;
                foreach (var apply_ruleset in apply_rulesets_random)
                {
                    probabilitygroup++;
                    List<(string Key, double? Value)> list = new List<(string Key, double? Value)>();
                    if (apply_ruleset is Newtonsoft.Json.Linq.JArray)
                    {
                        foreach (var apply_ruleset_item in apply_ruleset)
                        {
                            var x = (Newtonsoft.Json.Linq.JProperty)apply_ruleset_item.ToList()[0];
                            if (x.Value.Type == Newtonsoft.Json.Linq.JTokenType.Float)
                                list.Add((x.Name, (double)x.Value));
                            else if (x.Value.Type == Newtonsoft.Json.Linq.JTokenType.String && ((string)x.Value).ToLower() == "auto")
                                list.Add((x.Name, null));
                            else
                                throw new Exception($"apply_rulesets_random in {filename} for item {x.Name} has an invalid value. Must be a number between 0 and 1, or \"auto\"");
                        }
                    }
                    else
                    {
                        var apply_ruleset_item = apply_ruleset;
                        var x = (Newtonsoft.Json.Linq.JProperty)apply_ruleset_item.ToList()[0];
                        if (x.Value.Type == Newtonsoft.Json.Linq.JTokenType.Float)
                            list.Add((x.Name, (double)x.Value));
                        else if (x.Value.Type == Newtonsoft.Json.Linq.JTokenType.String && ((string)x.Value).ToLower() == "auto")
                            list.Add((x.Name, null));
                        else
                            throw new Exception($"apply_rulesets_random in {filename} for item {x.Name} has an invalid value. Must be a number between 0 and 1, or \"auto\"");
                    }
                    //var dict = apply_ruleset.ToObject<Dictionary<string, double?>>();
                    // var list = dict.ToList().Select(x => (x.Key, x.Value)).ToList();

                    //Ensure that all probabilities go up in order
                    double current = 0;
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i].Value.HasValue && list[i].Value <= current)
                            throw new Exception($"apply_rulesets_random in {filename} for item {list[i].Key} must have a value greater than the previous item (or greater than 0 if the first item).");
                        current = list[i].Value ?? current;
                    }
                    //Fill in missing values by applying a gradual increase to the next non null value
                    for (int i = 0; i < list.Count; i++)
                    {
                        var p = list[i].Value;
                        if (p == null)
                        {
                            double min = i == 0 ? 0 : list[i - 1].Value.Value;
                            int numToFill = 0;
                            double? max = null;
                            for (int j = i + 1; j < list.Count && max == null; j++, numToFill++)
                                max = list[j].Value;
                            if (max == null)
                                max = 1.0;
                            double delta = (max.Value - min) / (numToFill + 1);
                            for (int n = 0; n < numToFill; n++, i++)
                                list[i] = i == 0 ? (list[i].Key, delta) : (list[i].Key, list[i - 1].Value + delta);
                            if (numToFill == 0)
                                list[i] = (list[i].Key, max);
                        }
                    }
                    foreach (var item in list)
                    {
                        var link = new DBOld.RealmRulesetLinks();
                        link.Realm = realm;
                        link.Order = ++order;
                        link.ProbabilityGroup = probabilitygroup;
                        link.Probability = item.Value.Value;
                        link.LinkType = (ushort)RealmRulesetLinkType.apply_after_inherit;
                        link.Import_RulesetToApply = item.Key;
                        links.Add(link);
                    }
                }
            }
            return new RealmToImport()
            {
                Realm = realm,
                Links = links
            };
            // Command.Handlers.CommandHandlerHelper.WriteOutputError(session, $"Error importing json file {filename}. Exception: {ex.Message}.");
            //return null;
        }
    }
}
