using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModHelpers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets;
using static FeatureDefinitionSavingThrowAffinity;

using Helpers = SolastaModHelpers.Helpers;
using NewFeatureDefinitions = SolastaModHelpers.NewFeatureDefinitions;
using ExtendedEnums = SolastaModHelpers.ExtendedEnums;

namespace SolastaBarbarianClass
{
    internal class BarbarianClassBuilder : CharacterClassDefinitionBuilder
    {
        const string BarbarianClassName = "BarbarianClass";
        const string BarbarianClassNameGuid = "c7c54eb7-ddd0-430e-a7b3-74ff6594b7a4";
        const string BarbarianClassSubclassesGuid = "7f3de316-e5fc-491b-bf9d-a583ff81c2d2";

        static public NewFeatureDefinitions.ArmorClassStatBonus unarmored_defense;
        static public NewFeatureDefinitions.PowerUsableOnlyInBattle rage_power; 
        //Paths: Berserker, War shaman, frozen fury
        //unarmored defense
        //rage
        //reckless attack
        //danger sense
        //fast movement
        //danger sense
        //feral instinct
        //brutal critical

        static public RuleDefinitions.DieType[] inspiration_dice = new RuleDefinitions.DieType[] { RuleDefinitions.DieType.D6, RuleDefinitions.DieType.D8, RuleDefinitions.DieType.D10, RuleDefinitions.DieType.D12 };
        static public CharacterClassDefinition barbarian_class;
        static public Dictionary<RuleDefinitions.DieType, FeatureDefinitionPower> inspiration_powers = new Dictionary<RuleDefinitions.DieType, FeatureDefinitionPower>();
        static public FeatureDefinition font_of_inspiration;
        static public FeatureDefinitionPointPool expertise;
        static public FeatureDefinitionAbilityCheckAffinity jack_of_all_trades;
        static public Dictionary<RuleDefinitions.DieType, NewFeatureDefinitions.FeatureDefinitionExtraHealingDieOnShortRest> song_of_rest = new Dictionary<RuleDefinitions.DieType, NewFeatureDefinitions.FeatureDefinitionExtraHealingDieOnShortRest>();
        static public SpellListDefinition Barbarian_spelllist;
        static public SpellListDefinition magical_secrets_spelllist;
        static public NewFeatureDefinitions.FeatureDefinitionExtraSpellSelection magical_secrets;
        static public NewFeatureDefinitions.FeatureDefinitionExtraSpellSelection magical_secrets14;
        static public NewFeatureDefinitions.FeatureDefinitionExtraSpellSelection magical_secrets18;
        static public FeatureDefinitionPower countercharm;

        static public Dictionary<RuleDefinitions.DieType, FeatureDefinitionFeatureSet> cutting_words = new Dictionary<RuleDefinitions.DieType, FeatureDefinitionFeatureSet>();

        static public FeatureDefinitionPointPool lore_college_bonus_proficiencies;
        static public NewFeatureDefinitions.FeatureDefinitionExtraSpellSelection additional_magical_secrets;

        static public FeatureDefinitionFeatureSet virtue_college_bonus_proficiencies;
        static public Dictionary<RuleDefinitions.DieType, NewFeatureDefinitions.FeatureDefinitionReactionPowerOnAttackAttempt> music_of_spheres
            = new Dictionary<RuleDefinitions.DieType, NewFeatureDefinitions.FeatureDefinitionReactionPowerOnAttackAttempt>();
        static public FeatureDefinitionAttributeModifier virtue_college_extra_attack;


        static public FeatureDefinitionFeatureSet nature_college_bonus_proficiencies;
        static public FeatureDefinitionFeatureSet nature_college_extra_cantrip;
        static public FeatureDefinitionFeatureSet natural_focus;
        static public FeatureDefinition environmental_magical_secrets;

        protected BarbarianClassBuilder(string name, string guid) : base(name, guid)
        {
            var fighter = DatabaseHelper.CharacterClassDefinitions.Fighter;
            barbarian_class = Definition;
            Definition.GuiPresentation.Title = "Class/&BarbarianClassTitle";
            Definition.GuiPresentation.Description = "Class/&BarbarianClassDescription";
            Definition.GuiPresentation.SetSpriteReference(fighter.GuiPresentation.SpriteReference);

            Definition.SetClassAnimationId(AnimationDefinitions.ClassAnimationId.Fighter);
            Definition.SetClassPictogramReference(fighter.ClassPictogramReference);
            Definition.SetDefaultBattleDecisions(fighter.DefaultBattleDecisions);
            Definition.SetHitDice(RuleDefinitions.DieType.D12);
            Definition.SetIngredientGatheringOdds(fighter.IngredientGatheringOdds);
            Definition.SetRequiresDeity(false);

            Definition.AbilityScoresPriority.Clear();
            Definition.AbilityScoresPriority.AddRange(new List<string> {Helpers.Stats.Strength,
                                                                        Helpers.Stats.Constitution,
                                                                        Helpers.Stats.Dexterity,
                                                                        Helpers.Stats.Charisma,
                                                                        Helpers.Stats.Wisdom,
                                                                        Helpers.Stats.Intelligence});

            Definition.FeatAutolearnPreference.AddRange(fighter.FeatAutolearnPreference);
            Definition.PersonalityFlagOccurences.AddRange(fighter.PersonalityFlagOccurences);

            Definition.SkillAutolearnPreference.Clear();
            Definition.SkillAutolearnPreference.AddRange(new List<string> { Helpers.Skills.Athletics,
                                                                            Helpers.Skills.Intimidation,
                                                                            Helpers.Skills.Perception,
                                                                            Helpers.Skills.Nature,
                                                                            Helpers.Skills.Survival,
                                                                            Helpers.Skills.AnimalHandling,
                                                                            Helpers.Skills.Acrobatics,
                                                                            Helpers.Skills.Stealth });

            Definition.ToolAutolearnPreference.Clear();
            Definition.ToolAutolearnPreference.AddRange(new List<string> { Helpers.Tools.SmithTool, Helpers.Tools.HerbalismKit });


            Definition.EquipmentRows.AddRange(fighter.EquipmentRows);
            Definition.EquipmentRows.Clear();

            this.AddEquipmentRow(new List<CharacterClassDefinition.HeroEquipmentOption>
                                    {
                                        EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.Greataxe, EquipmentDefinitions.OptionWeapon, 1),
                                    },
                                new List<CharacterClassDefinition.HeroEquipmentOption>
                                    {
                                        EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.Greataxe, EquipmentDefinitions.OptionWeaponMartialChoice, 1),
                                    }
            );
            this.AddEquipmentRow(new List<CharacterClassDefinition.HeroEquipmentOption>
                                    {
                                        EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.Handaxe, EquipmentDefinitions.OptionWeapon, 2),
                                    },
                                new List<CharacterClassDefinition.HeroEquipmentOption>
                                    {
                                        EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.Mace, EquipmentDefinitions.OptionWeaponSimpleChoice, 1),
                                    }
            );
            this.AddEquipmentRow(new List<CharacterClassDefinition.HeroEquipmentOption>
                                    {
                                        EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.ExplorerPack, EquipmentDefinitions.OptionStarterPack, 1),
                                    }
            );

            this.AddEquipmentRow(new List<CharacterClassDefinition.HeroEquipmentOption>
            {
                EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.Javelin, EquipmentDefinitions.OptionWeapon, 4),
            });

            var saving_throws = Helpers.ProficiencyBuilder.CreateSavingthrowProficiency("BarbarianSavingthrowProficiency",
                                                                                        "",
                                                                                        Helpers.Stats.Strength, Helpers.Stats.Constitution);

            var armor_proficiency = Helpers.ProficiencyBuilder.createCopy("BarbarianArmorProficiency",
                                                                          "",
                                                                          "Feature/&BarbarianArmorProficiencyTitle",
                                                                          "",
                                                                          DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyClericArmor
                                                                          );

            var weapon_proficiency = Helpers.ProficiencyBuilder.createCopy("BarbarianWeaponProficiency",
                                                                          "",
                                                                          "Feature/&BarbarianWeaponProficiencyTitle",
                                                                          "",
                                                                          DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyFighterWeapon
                                                                          );

            var skills = Helpers.PoolBuilder.createSkillProficiency("BarbarianSkillProficiency",
                                                                    "",
                                                                    "Feature/&BarbarianClassSkillPointPoolTitle",
                                                                    "Feature/&SkillGainChoicesPluralDescription",
                                                                    3,
                                                                    Helpers.Skills.AnimalHandling, Helpers.Skills.Athletics, Helpers.Skills.Intimidation, 
                                                                    Helpers.Skills.Nature, Helpers.Skills.Perception, Helpers.Skills.Survival
                                                                    );


            createUnarmoredDefense();
            createRage();
            Definition.FeatureUnlocks.Clear();
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(saving_throws, 1));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(armor_proficiency, 1));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(weapon_proficiency, 1));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(skills, 1));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(rage_power, 1));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(unarmored_defense, 1));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 4));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 8));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 12));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 16));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 19));

            var subclassChoicesGuiPresentation = new GuiPresentation();
            subclassChoicesGuiPresentation.Title = "Subclass/&BarbarianSubclassCollegeTitle";
            subclassChoicesGuiPresentation.Description = "Subclass/&BarbarianSubclassCollegeDescription";
            BarbarianFeatureDefinitionSubclassChoice = this.BuildSubclassChoice(3, "College", false, "SubclassChoiceBarbarianSpecialistArchetypes", subclassChoicesGuiPresentation, BarbarianClassSubclassesGuid);
        }


        static void createRage()
        {
            string rage_title_string = "Feature/&BarbarianClassRagePowerTitle";
            string rage_description_string = "Feature/&BarbarianClassRagePowerDescription";
            string rage_condition_string = "Rules/&BarbarianClassRageCondition";


            var condition_can_continue_rage = Helpers.ConditionBuilder.createConditionWithInterruptions("BarbarianClassCanContinueRageCondition",
                                                                                      "",
                                                                                      Common.common_no_title,
                                                                                      Common.common_no_title,
                                                                                      Common.common_no_icon,
                                                                                      DatabaseHelper.ConditionDefinitions.ConditionHeroism,
                                                                                      new RuleDefinitions.ConditionInterruption[] { }
                                                                                      );
            condition_can_continue_rage.SetSilentWhenAdded(true);
            condition_can_continue_rage.SetSilentWhenRemoved(true);

            var damage_bonus = Helpers.CopyFeatureBuilder<FeatureDefinitionAdditionalDamage>.createFeatureCopy("BarbarianClassRageDamageBonus",
                                                                                                               "",
                                                                                                               Common.common_no_title,
                                                                                                               Common.common_no_title,
                                                                                                               null,
                                                                                                               DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageBracersOfArchery,
                                                                                                               d =>
                                                                                                               {
                                                                                                                   d.SetRequiredProperty(RuleDefinitions.AdditionalDamageRequiredProperty.MeleeWeapon);
                                                                                                                   d.SetDamageDieType(RuleDefinitions.DieType.D1);
                                                                                                                   d.SetDamageDiceNumber(2);
                                                                                                                   d.SetNotificationTag("Enraged");
                                                                                                               }
                                                                                                               );

            var rage_condition = Helpers.ConditionBuilder.createConditionWithInterruptions("BarbarianClassRageCondition",
                                                                                      "",
                                                                                      rage_condition_string,
                                                                                      rage_description_string,
                                                                                      null,
                                                                                      DatabaseHelper.ConditionDefinitions.ConditionHeroism,
                                                                                      new RuleDefinitions.ConditionInterruption[] {},
                                                                                      DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityBludgeoningResistance,
                                                                                      DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPiercingResistance,
                                                                                      DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinitySlashingResistance,
                                                                                      DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionBullsStrength,
                                                                                      damage_bonus
                                                                                      );

            var rage_watcher = Helpers.FeatureBuilder<NewFeatureDefinitions.RageWatcher>.createFeature("BarbarianClassRageAttackWatcher",
                                                                                           "",
                                                                                           Common.common_no_title,
                                                                                           Common.common_no_title,
                                                                                           null,
                                                                                           r =>
                                                                                           {
                                                                                               r.requiredCondition = condition_can_continue_rage;
                                                                                               r.conditionToRemove = rage_condition;
                                                                                           }
                                                                                           );

            rage_condition.Features.Add(rage_watcher);

            var effect = new EffectDescription();
            effect.Copy(DatabaseHelper.FeatureDefinitionPowers.PowerDomainBattleHeraldOfBattle.EffectDescription);
            effect.SetRangeType(RuleDefinitions.RangeType.Self);
            effect.SetRangeParameter(1);
            effect.DurationParameter = 1;
            effect.DurationType = RuleDefinitions.DurationType.Minute;
            effect.EffectForms.Clear();
            effect.SetTargetType(RuleDefinitions.TargetType.Self);

            var effect_form = new EffectForm();
            effect_form.ConditionForm = new ConditionForm();
            effect_form.FormType = EffectForm.EffectFormType.Condition;
            effect_form.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            effect_form.ConditionForm.ConditionDefinition = rage_condition;
            effect.EffectForms.Add(effect_form);

            rage_power = Helpers.GenericPowerBuilder<NewFeatureDefinitions.PowerUsableOnlyInBattle>
                                                      .createPower("BarbarianClassRagePower",
                                                         "",
                                                         rage_title_string,
                                                         rage_description_string,
                                                         DatabaseHelper.FeatureDefinitionPowers.PowerDomainBattleHeraldOfBattle.GuiPresentation.SpriteReference,
                                                         effect,
                                                         RuleDefinitions.ActivationTime.BonusAction,
                                                         2,
                                                         RuleDefinitions.UsesDetermination.Fixed,
                                                         RuleDefinitions.RechargeRate.LongRest
                                                         );

            rage_power.SetShortTitleOverride(rage_title_string);
        }


        static void createUnarmoredDefense()
        {
            unarmored_defense = Helpers.FeatureBuilder<NewFeatureDefinitions.ArmorClassStatBonus>.createFeature("BarbarianClassUnarmoredDefense",
                                                                                                                "",
                                                                                                                "Feature/&BarbarianClassUnarmoredDefenseTitle",
                                                                                                                "Feature/&BarbarianClassUnarmoredDefenseDescription",
                                                                                                                null,
                                                                                                                a =>
                                                                                                                {
                                                                                                                    a.armorAllowed = false;
                                                                                                                    a.shieldAlowed = true;
                                                                                                                    a.stat = Helpers.Stats.Constitution;
                                                                                                                    a.forbiddenConditions = new List<ConditionDefinition>
                                                                                                                    {
                                                                                                                        DatabaseHelper.ConditionDefinitions.ConditionBarkskin,
                                                                                                                        DatabaseHelper.ConditionDefinitions.ConditionMagicallyArmored
                                                                                                                    };
                                                                                                                }
                                                                                                                );
        }


        static CharacterSubclassDefinition createNatureCollege()
        {
            createNatureCollegeBonusProficienies();
            createNatureCollegeExtraCantrip();



            var gui_presentation = new GuiPresentationBuilder(
                    "Subclass/&BarbarianSubclassCollegeOfNatureDescription",
                    "Subclass/&BarbarianSubclassCollegeOfNatureTitle")
                    .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.TraditionGreenmage.GuiPresentation.SpriteReference)
                    .Build();

            CharacterSubclassDefinition definition = new CharacterSubclassDefinitionBuilder("BarbarianSubclassCollegeOfNature", "58a3447b-a154-4da2-9a56-fb38d5e6c0b4")
                                                                                            .SetGuiPresentation(gui_presentation)
                                                                                            .AddFeatureAtLevel(nature_college_bonus_proficiencies, 3)
                                                                                            .AddFeatureAtLevel(nature_college_extra_cantrip, 3)
                                                                                            .AddToDB();

            return definition;
        }


        static void createNatureCollegeExtraCantrip()
        {
            string title = "Feature/&BarbarianNatureSubclassBonusCantripTitle";
            string description = "Feature/&BarbarianNatureSubclassBonusCantripDescription";

            var cantrips = new SpellDefinition[] { DatabaseHelper.SpellDefinitions.Guidance, DatabaseHelper.SpellDefinitions.PoisonSpray, DatabaseHelper.SpellDefinitions.Resistance };

            List<FeatureDefinition> learn_features = new List<FeatureDefinition>();

            foreach (var c in cantrips)
            {
                var feature = Helpers.BonusCantripsBuilder.createLearnBonusCantrip(c.name + "BarbarianNatureSubclassBonusCantrip",
                                                                                   "",
                                                                                   c.GuiPresentation.Title,
                                                                                   c.GuiPresentation.Description,
                                                                                   c);
                learn_features.Add(feature);
            }

            nature_college_extra_cantrip = Helpers.FeatureSetBuilder.createFeatureSet("BarbarianNatureSubclassBonusCantrip",
                                                                                "",
                                                                                title,
                                                                                description,
                                                                                false,
                                                                                FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion,
                                                                                false,
                                                                                learn_features.ToArray()
                                                                                );
        }


        static void createNatureCollegeBonusProficienies()
        {
            var tools_proficiency = Helpers.ProficiencyBuilder.CreateToolsProficiency("BarbarianNatureSubclassToolsProficiency",
                                                                                      "",
                                                                                      Common.common_no_title,
                                                                                      Helpers.Tools.HerbalismKit
                                                                                      );

            var skills = Helpers.PoolBuilder.createSkillProficiency("BarbarianNatureSubclassSkillsProficiency",
                                                                    "",
                                                                    Common.common_no_title,
                                                                    Common.common_no_title,
                                                                    2,
                                                                    Helpers.Skills.Nature, Helpers.Skills.Medicine, Helpers.Skills.AnimalHandling, Helpers.Skills.Survival);

            nature_college_bonus_proficiencies = Helpers.FeatureSetBuilder.createFeatureSet("BarbarianNatureSubclassBonusProficiencies",
                                                                                            "",
                                                                                            "Feature/&BarbarianNatureSubclassBonusProficiencieslTitle",
                                                                                            "Feature/&BarbarianNatureSubclassBonusProficiencieslDescription",
                                                                                            false,
                                                                                            FeatureDefinitionFeatureSet.FeatureSetMode.Union,
                                                                                            false,
                                                                                            skills,
                                                                                            tools_proficiency
                                                                                            );
        }


        static CharacterSubclassDefinition createVirtueCollege()
        {
            createVirtueCollegeProficiencies();

            var gui_presentation = new GuiPresentationBuilder(
                    "Subclass/&BarbarianSubclassCollegeOfVirtueDescription",
                    "Subclass/&BarbarianSubclassCollegeOfVirtueTitle")
                    .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.OathOfTirmar.GuiPresentation.SpriteReference)
                    .Build();

            CharacterSubclassDefinition definition = new CharacterSubclassDefinitionBuilder("BarbarianSubclassCollegeOfVirtue", "278c2e79-d301-4c35-974f-0ee51e7d4eb3")
                    .SetGuiPresentation(gui_presentation)
                    .AddFeatureAtLevel(virtue_college_bonus_proficiencies, 3)
                    .AddToDB();

            return definition;
        }


        static void createVirtueCollegeProficiencies()
        {
            var armor_proficiency = Helpers.ProficiencyBuilder.CreateArmorProficiency("BarbarianVirtueSubclassArmorProficiency",
                                                              "",
                                                              Common.common_no_title,
                                                              Common.common_no_title,
                                                              Helpers.ArmorProficiencies.MediumArmor,
                                                              Helpers.ArmorProficiencies.Shield
                                                              );

            var wis_proficiency = Helpers.ProficiencyBuilder.CreateSavingthrowProficiency("BarbarianVirtueSubclassWisSavingthrowsProficiency",
                                                                                          "",
                                                                                          Helpers.Stats.Wisdom
                                                                                          );

            virtue_college_bonus_proficiencies = Helpers.FeatureSetBuilder.createFeatureSet("BarbarianVirtueSubclassBonusProficiency",
                                                                                            "",
                                                                                            "Feature/&BarbarianVirtueSublclassBonusProficiencieslTitle",
                                                                                            "Feature/&BarbarianVirtueSublclassBonusProficiencieslDescription",
                                                                                            false,
                                                                                            FeatureDefinitionFeatureSet.FeatureSetMode.Union,
                                                                                            false,
                                                                                            armor_proficiency,
                                                                                            wis_proficiency
                                                                                            );

        }



        static CharacterSubclassDefinition createLoreCollege()
        {
            createLoreCollegeBonusProficiencies();

            var gui_presentation = new GuiPresentationBuilder(
                    "Subclass/&BarbarianSubclassCollegeOfLoreDescription",
                    "Subclass/&BarbarianSubclassCollegeOfLoreTitle")
                    .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.TraditionLoremaster.GuiPresentation.SpriteReference)
                    .Build();

            CharacterSubclassDefinition definition = new CharacterSubclassDefinitionBuilder("BarbarianSubclassCollegeOfLore", "a6ba03a4-2bbf-488c-97c0-d42d43c8afe3")
                    .SetGuiPresentation(gui_presentation)
                    .AddFeatureAtLevel(lore_college_bonus_proficiencies, 3)
                    .AddToDB();

            return definition;
        }


        static void createLoreCollegeBonusProficiencies()
        {
            lore_college_bonus_proficiencies = Helpers.PoolBuilder.createSkillProficiency("BarbarianLoreSubclassSkillProficiency",
                                                        "",
                                                        "Feature/&BarbarianLoreSublclassExtraSkillPointPoolTitle",
                                                        "Feature/&BarbarianLoreSublclassExtraSkillPointPoolDescription",
                                                        3,
                                                        Helpers.Skills.getAllSkills());
        }

        public static void BuildAndAddClassToDB()
        {
            var BarbarianClass = new BarbarianClassBuilder(BarbarianClassName, BarbarianClassNameGuid).AddToDB();
            BarbarianClass.FeatureUnlocks.Sort(delegate (FeatureUnlockByLevel a, FeatureUnlockByLevel b)
                                          {
                                              return a.Level - b.Level;
                                          }
                                         );

            BarbarianFeatureDefinitionSubclassChoice.Subclasses.Add(createLoreCollege().Name);
            BarbarianFeatureDefinitionSubclassChoice.Subclasses.Add(createVirtueCollege().Name);
            BarbarianFeatureDefinitionSubclassChoice.Subclasses.Add(createNatureCollege().Name);
        }

        private static FeatureDefinitionSubclassChoice BarbarianFeatureDefinitionSubclassChoice;
    }
}
