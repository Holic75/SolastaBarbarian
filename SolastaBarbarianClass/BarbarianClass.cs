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

        static Dictionary<int, int> rage_bonus_damage_level_map = new Dictionary<int, int> { { 2, 1 }, { 3, 9 }, { 4, 16 } };
        static List<int> rage_uses_increase_levels = new List<int> { 3, 6, 12, 17 };
        static public NewFeatureDefinitions.ArmorClassStatBonus unarmored_defense;
        static public NewFeatureDefinitions.SpellcastingForbidden rage_spellcasting_forbiden;
        static public Dictionary<int, NewFeatureDefinitions.PowerWithRestrictions> rage_powers = new Dictionary<int, NewFeatureDefinitions.PowerWithRestrictions>();
        static public NewFeatureDefinitions.PowerWithRestrictions reckless_attack_power;
        static public FeatureDefinitionFeatureSet reckless_attack;
        static public NewFeatureDefinitions.SavingthrowAffinityUnderRestriction danger_sense;
        static public Dictionary<int, NewFeatureDefinitions.IncreaseNumberOfPowerUses> rage_power_extra_use = new Dictionary<int, NewFeatureDefinitions.IncreaseNumberOfPowerUses>();
        static public FeatureDefinitionAttributeModifier extra_attack;
        static public NewFeatureDefinitions.MovementBonusWithRestrictions fast_movement;
        static public FeatureDefinitionFeatureSet feral_instinct;
        static public NewFeatureDefinitions.WeaponDamageDiceIncreaseOnCriticalHit brutal_critical;
        //Frozen Fury
        static NewFeatureDefinitions.ApplyPowerOnTurnEndBasedOnClassLevel frozen_fury_rage_feature;
        static public FeatureDefinition frozen_fury;
        static public NewFeatureDefinitions.ArmorBonusAgainstAttackType frigid_body;
        static public FeatureDefinitionFeatureSet numb;
        //War Shaman
        static public SpellListDefinition war_shaman_spelllist;
        static public FeatureDefinitionCastSpell war_shaman_spellcasting;
        static public Dictionary<int, NewFeatureDefinitions.PowerWithRestrictions> share_rage_powers = new Dictionary<int, NewFeatureDefinitions.PowerWithRestrictions>();
        static public FeatureDefinition ragecaster;


        //Berserker
        //frenzy
        static public NewFeatureDefinitions.PowerWithRestrictions frenzy;
        static public ConditionDefinition exhausted_after_frenzy_condition;
        static public FeatureDefinitionFeatureSet mindless_rage;
        static public FeatureDefinitionPower intimidating_presence;
        static public FeatureDefinitionFeatureSet intimidating_presence_feature;

        static public CharacterClassDefinition barbarian_class;


        protected BarbarianClassBuilder(string name, string guid) : base(name, guid)
        {
            var barbarian_class_image = SolastaModHelpers.CustomIcons.Tools.storeCustomIcon("BarbarianClassImage",
                                                                                                $@"{UnityModManagerNet.UnityModManager.modsPath}/SolastaBarbarianClass/Sprites/BarbarianClass.png",
                                                                                                1024, 576);

            var fighter = DatabaseHelper.CharacterClassDefinitions.Fighter;
            barbarian_class = Definition;
            Definition.GuiPresentation.Title = "Class/&BarbarianClassTitle";
            Definition.GuiPresentation.Description = "Class/&BarbarianClassDescription";
            Definition.GuiPresentation.SetSpriteReference(barbarian_class_image);

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
                                                                    2,
                                                                    Helpers.Skills.AnimalHandling, Helpers.Skills.Athletics, Helpers.Skills.Intimidation, 
                                                                    Helpers.Skills.Nature, Helpers.Skills.Perception, Helpers.Skills.Survival
                                                                    );
            createUnarmoredDefense();
            createRage();
            createRecklessAttack();
            createDangerSense();
            createFastMovement();
            createExtraAttack();
            createFeralInstinct();
            createBrutalCritical();
            Definition.FeatureUnlocks.Clear();
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(saving_throws, 1));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(armor_proficiency, 1));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(weapon_proficiency, 1));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(skills, 1));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(rage_powers[1], 1));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(unarmored_defense, 1));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(reckless_attack, 2));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(danger_sense, 2));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(rage_power_extra_use[3], 3));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 4));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(extra_attack, 5));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(fast_movement, 5));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(rage_power_extra_use[6], 6));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(feral_instinct, 7));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 8));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(rage_powers[9], 9));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(brutal_critical, 9));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 12));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(rage_power_extra_use[12], 12));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(brutal_critical, 13));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 16));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(rage_powers[16], 16));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(rage_power_extra_use[17], 17));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(brutal_critical, 17));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 19));

            var subclassChoicesGuiPresentation = new GuiPresentation();
            subclassChoicesGuiPresentation.Title = "Subclass/&BarbarianSubclassPrimalPathTitle";
            subclassChoicesGuiPresentation.Description = "Subclass/&BarbarianSubclassPrimalPathDescription";
            BarbarianFeatureDefinitionSubclassChoice = this.BuildSubclassChoice(3, "PrimalPath", false, "SubclassChoiceBarbarianSpecialistArchetypes", subclassChoicesGuiPresentation, BarbarianClassSubclassesGuid);
        }

        static void createBrutalCritical()
        {
            string brutal_critical_title_string = "Feature/&BarbarianClassBrutalCriticalTitle";
            string brutal_critical_description_string = "Feature/&BarbarianClassBrutalCriticalDescription";

            brutal_critical = Helpers.FeatureBuilder<NewFeatureDefinitions.WeaponDamageDiceIncreaseOnCriticalHit>.createFeature("BarbarianClassBrutalCritical",
                                                                                                                                "",
                                                                                                                                brutal_critical_title_string,
                                                                                                                                brutal_critical_description_string,
                                                                                                                                null,
                                                                                                                                b =>
                                                                                                                                {
                                                                                                                                    b.applyToMelee = true;
                                                                                                                                    b.applyToRanged = false;
                                                                                                                                    b.value = 1;
                                                                                                                                }
                                                                                                                               );
        }


        static void createFeralInstinct()
        {
            string feral_instinct_title_string = "Feature/&BarbarianClassFeralInstinctTitle";
            string feral_instinct_description_string = "Feature/&BarbarianClassFeralInstinctDescription";
            var cannot_be_surprised = Helpers.FeatureBuilder<FeatureDefinitionPerceptionAffinity>.createFeature("BarbarainClassCanNotBeSurprised",
                                                                                                                "",
                                                                                                                Common.common_no_title,
                                                                                                                Common.common_no_title,
                                                                                                                null,
                                                                                                                c =>
                                                                                                                {
                                                                                                                    c.SetCannotBeSurprised(true);
                                                                                                                }
                                                                                                                );
            feral_instinct = Helpers.FeatureSetBuilder.createFeatureSet("BarbarianClassFeralInstinct",
                                                                        "",
                                                                        feral_instinct_title_string,
                                                                        feral_instinct_description_string,
                                                                        false,
                                                                        FeatureDefinitionFeatureSet.FeatureSetMode.Union,
                                                                        false,
                                                                        cannot_be_surprised,
                                                                        DatabaseHelper.FeatureDefinitionCombatAffinitys.CombatAffinityEagerForBattle
                                                                        );
        }

        static void createExtraAttack()
        {
            extra_attack = Helpers.CopyFeatureBuilder<FeatureDefinitionAttributeModifier>.createFeatureCopy("BarbarianClassExtraAttack",
                                                                                                            "",
                                                                                                            "",
                                                                                                            "",
                                                                                                            null,
                                                                                                            DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierFighterExtraAttack
                                                                                                            );
        }


        static void createFastMovement()
        {
            string fast_movement_title_string = "Feature/&BarbarianClassFastMovementTitle";
            string fast_movement_description_string = "Feature/&BarbarianClassFastMovementDescription";

            fast_movement = Helpers.FeatureBuilder<NewFeatureDefinitions.MovementBonusWithRestrictions>.createFeature("BarbarianClassFastMovement",
                                                                                                                      "",
                                                                                                                      fast_movement_title_string,
                                                                                                                      fast_movement_description_string,
                                                                                                                      null,
                                                                                                                      a =>
                                                                                                                      {
                                                                                                                          a.restrictions = new List<NewFeatureDefinitions.IRestriction>()
                                                                                                                          {
                                                                                                                              new NewFeatureDefinitions.ArmorTypeRestriction(DatabaseHelper.ArmorCategoryDefinitions.HeavyArmorCategory, inverted: true)
                                                                                                                          };
                                                                                                                          a.modifiers = new List<FeatureDefinition> { DatabaseHelper.FeatureDefinitionMovementAffinitys.MovementAffinityLongstrider };
                                                                                                                      }
                                                                                                                      );
        }

        static void createDangerSense()
        {
            string danger_sense_title_string = "Feature/&BarbarianClassDangerSenseTitle";
            string danger_sense_description_string = "Feature/&BarbarianClassDangerSenseDescription";

            danger_sense = Helpers.FeatureBuilder<NewFeatureDefinitions.SavingthrowAffinityUnderRestriction>.createFeature("BarbarianClassDangerSenseFeature",
                                                                                                                            "",
                                                                                                                            danger_sense_title_string,
                                                                                                                            danger_sense_description_string,
                                                                                                                            Common.common_no_icon,
                                                                                                                            d =>
                                                                                                                            {
                                                                                                                                d.affinityGroups = new List<SavingThrowAffinityGroup>
                                                                                                                                {
                                                                                                                                    new SavingThrowAffinityGroup()
                                                                                                                                    {
                                                                                                                                        abilityScoreName = Helpers.Stats.Dexterity,
                                                                                                                                        affinity = RuleDefinitions.CharacterSavingThrowAffinity.Advantage,
                                                                                                                                    }
                                                                                                                                };
                                                                                                                                d.restrictions = new List<NewFeatureDefinitions.IRestriction>
                                                                                                                                {
                                                                                                                                    new NewFeatureDefinitions.NoConditionRestriction(DatabaseHelper.ConditionDefinitions.ConditionIncapacitated),
                                                                                                                                    new NewFeatureDefinitions.NoConditionRestriction(DatabaseHelper.ConditionDefinitions.ConditionBlinded),
                                                                                                                                    new NewFeatureDefinitions.NoConditionRestriction(DatabaseHelper.ConditionDefinitions.ConditionDeafened)
                                                                                                                                };
                                                                                                                            }
                                                                                                                            );
        }


        static void createRecklessAttack()
        {
            string reckless_attack_title_string = "Feature/&BarbarianClassRecklessAttackPowerTitle";
            string reckless_attack_description_string = "Feature/&BarbarianClassRecklessAttackPowerDescription";
            string reckless_condition_string = "Rules/&BarbarianClassRecklessCondition";

            var condition_attacked_this_turn = Helpers.ConditionBuilder.createConditionWithInterruptions("BarbarianClassAttackedThisTurnCondition",
                                                                                                         "",
                                                                                                         Common.common_no_title,
                                                                                                         Common.common_no_title,
                                                                                                         Common.common_no_icon,
                                                                                                         DatabaseHelper.ConditionDefinitions.ConditionHeroism,
                                                                                                         new RuleDefinitions.ConditionInterruption[] { }
                                                                                                        );
            condition_attacked_this_turn.SetSilentWhenAdded(true);
            condition_attacked_this_turn.SetSilentWhenRemoved(true);


            var effect_feature = Helpers.FeatureBuilder<NewFeatureDefinitions.RecklessAttack>.createFeature("BarbarianClassRecklessAttackEffectFeature",
                                                                                                            "",
                                                                                                            reckless_attack_title_string,
                                                                                                            reckless_attack_description_string,
                                                                                                            Common.common_no_icon,
                                                                                                            d =>
                                                                                                            {
                                                                                                                d.attackStat = Helpers.Stats.Strength;
                                                                                                            }
                                                                                                            );

            var condition = Helpers.ConditionBuilder.createConditionWithInterruptions("BarbarianClassRecklessAttackCondition",
                                                                                      "",
                                                                                      reckless_condition_string,
                                                                                      "Feature/&ConditionBarbarianClassRecklessAttackPowerDescription",
                                                                                      null,
                                                                                      DatabaseHelper.ConditionDefinitions.ConditionHeraldOfBattle,
                                                                                      new RuleDefinitions.ConditionInterruption[] { },
                                                                                      effect_feature
                                                                                      );
            condition.SetTurnOccurence(RuleDefinitions.TurnOccurenceType.StartOfTurn);
            condition.SetConditionType(RuleDefinitions.ConditionType.Neutral);
            //condition.SetSpecialDuration(true);
            // condition.SetDurationParameter(1);
            // condition.SetDurationParameterDie(RuleDefinitions.DieType.D1);
            //  condition.SetDurationType(RuleDefinitions.DurationType.Round);

            var reckless_attack_watcher = Helpers.FeatureBuilder<NewFeatureDefinitions.InitiatorApplyConditionOnAttackToAttackerUntilTurnStart>.createFeature("BarbarianClassAttackedMark",
                                                                                                       "",
                                                                                                       Common.common_no_title,
                                                                                                       Common.common_no_title,
                                                                                                       null,
                                                                                                       r =>
                                                                                                       {
                                                                                                           r.condition = condition_attacked_this_turn;
                                                                                                           r.extraConditionsToRemove = new List<ConditionDefinition>() { condition };
                                                                                                       }
                                                                                                       );

            var effect = new EffectDescription();
            effect.Copy(DatabaseHelper.SpellDefinitions.Heroism.EffectDescription);
            effect.SetRangeType(RuleDefinitions.RangeType.Self);
            effect.SetRangeParameter(1);
            effect.DurationParameter = 1;
            effect.DurationType = RuleDefinitions.DurationType.Round;
            effect.EffectForms.Clear();
            effect.SetTargetType(RuleDefinitions.TargetType.Self);

            var effect_form = new EffectForm();
            effect_form.ConditionForm = new ConditionForm();
            effect_form.FormType = EffectForm.EffectFormType.Condition;
            effect_form.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            effect_form.ConditionForm.ConditionDefinition = condition;
            effect.EffectForms.Add(effect_form);

            reckless_attack_power = Helpers.GenericPowerBuilder<NewFeatureDefinitions.PowerWithRestrictions>
                                                      .createPower("BarbarianClassRecklessAttackPower",
                                                         "",
                                                         reckless_attack_title_string,
                                                         reckless_attack_description_string,
                                                         DatabaseHelper.FeatureDefinitionPowers.PowerDomainBattleDecisiveStrike.GuiPresentation.SpriteReference,
                                                         effect,
                                                         RuleDefinitions.ActivationTime.NoCost,
                                                         1,
                                                         RuleDefinitions.UsesDetermination.Fixed,
                                                         RuleDefinitions.RechargeRate.AtWill
                                                         );
            reckless_attack_power.restrictions = new List<NewFeatureDefinitions.IRestriction>()
            {
                new NewFeatureDefinitions.InBattleRestriction(),
                new NewFeatureDefinitions.NoConditionRestriction(condition),
                new NewFeatureDefinitions.NoConditionRestriction(condition_attacked_this_turn)
            };

            reckless_attack_power.SetShortTitleOverride(reckless_attack_title_string);
            reckless_attack = Helpers.FeatureSetBuilder.createFeatureSet("BarbarianClassRecklessAttackFeatureSet",
                                                                         "",
                                                                         reckless_attack_title_string,
                                                                         reckless_attack_description_string,
                                                                         false,
                                                                         FeatureDefinitionFeatureSet.FeatureSetMode.Union,
                                                                         false,
                                                                         reckless_attack_power,
                                                                         reckless_attack_watcher
                                                                         );
        }

        static void createRage()
        {
            string rage_title_string = "Feature/&BarbarianClassRagePowerTitle";
            string rage_description_string = "Feature/&BarbarianClassRagePowerDescription";
            string rage_condition_string = "Rules/&BarbarianClassRageCondition";

            rage_spellcasting_forbiden = Helpers.FeatureBuilder<NewFeatureDefinitions.SpellcastingForbidden>.createFeature("BarbarianClassRageSpellcastingForbidden",
                                                                                                                           "",
                                                                                                                           Common.common_no_title,
                                                                                                                           Common.common_no_title,
                                                                                                                           Common.common_no_icon,
                                                                                                                           r =>
                                                                                                                           {
                                                                                                                               r.spellcastingExceptionFeatures = new List<FeatureDefinition>();
                                                                                                                               r.concentrationExceptionFeatures = new List<FeatureDefinition>();
                                                                                                                               r.forbidConcentration = true;
                                                                                                                           }
                                                                                                                           );

            var condition_can_continue_rage = Helpers.ConditionBuilder.createConditionWithInterruptions("BarbarianClassCanContinueRageCondition",
                                                                                                          "",
                                                                                                          "Rules/&BarbarianClassCanContinueRageCondition",
                                                                                                          Common.common_no_title,
                                                                                                          Common.common_no_icon,
                                                                                                          DatabaseHelper.ConditionDefinitions.ConditionHeroism,
                                                                                                          new RuleDefinitions.ConditionInterruption[] { }
                                                                                                          );
            condition_can_continue_rage.SetSilentWhenAdded(true);
            condition_can_continue_rage.SetSilentWhenRemoved(true);

            NewFeatureDefinitions.PowerWithRestrictions previous_power = null;

            foreach (var kv in rage_bonus_damage_level_map)
            {
                var damage_bonus = Helpers.FeatureBuilder<NewFeatureDefinitions.WeaponDamageBonusWithSpecificStat>.createFeature("BarbarianClassRageDamageBonus" + kv.Value.ToString(),
                                                                                                                   "",
                                                                                                                   rage_condition_string,
                                                                                                                   rage_condition_string,
                                                                                                                   null,
                                                                                                                   d =>
                                                                                                                   {
                                                                                                                       d.value = kv.Key;
                                                                                                                       d.attackStat = Helpers.Stats.Strength;
                                                                                                                   }
                                                                                                                   );

                var rage_condition = Helpers.ConditionBuilder.createConditionWithInterruptions("BarbarianClassRageCondition" + kv.Value.ToString(),
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
                                                                                          damage_bonus,
                                                                                          rage_spellcasting_forbiden
                                                                                          );
                rage_condition.SetPossessive(false);
                

                var rage_watcher = Helpers.FeatureBuilder<NewFeatureDefinitions.RageWatcher>.createFeature("BarbarianClassRageAttackWatcher" + kv.Value.ToString(),
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
                effect.SetRecurrentEffect(RuleDefinitions.RecurrentEffect.No);

                var effect_form = new EffectForm();
                effect_form.ConditionForm = new ConditionForm();
                effect_form.FormType = EffectForm.EffectFormType.Condition;
                effect_form.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
                effect_form.ConditionForm.ConditionDefinition = rage_condition;
                effect.EffectForms.Add(effect_form);

                var rage_power = Helpers.GenericPowerBuilder<NewFeatureDefinitions.PowerWithRestrictions>
                                                          .createPower("BarbarianClassRagePower" + kv.Value.ToString(),
                                                             "",
                                                             Helpers.StringProcessing.appendToString(rage_title_string,
                                                                                                             rage_title_string + kv.Value.ToString(),
                                                                                                             $" (+{kv.Key})"),
                                                             rage_description_string,
                                                             //DatabaseHelper.FeatureDefinitionPowers.PowerDomainBattleDivineWrath.GuiPresentation.SpriteReference,
                                                             DatabaseHelper.SpellDefinitions.Heroism.GuiPresentation.SpriteReference,
                                                             effect,
                                                             RuleDefinitions.ActivationTime.BonusAction,
                                                             2 + rage_uses_increase_levels.Count(l => l < kv.Value),
                                                             RuleDefinitions.UsesDetermination.Fixed,
                                                             RuleDefinitions.RechargeRate.LongRest
                                                             );
                rage_power.restrictions = new List<NewFeatureDefinitions.IRestriction>()
                {
                    new NewFeatureDefinitions.InBattleRestriction(),
                    new NewFeatureDefinitions.NoConditionRestriction(condition_can_continue_rage),
                    new NewFeatureDefinitions.ArmorTypeRestriction(DatabaseHelper.ArmorCategoryDefinitions.HeavyArmorCategory, inverted: true)
                };

                rage_power.SetShortTitleOverride(rage_title_string);
                if (previous_power != null)
                {
                    rage_power.SetOverriddenPower(previous_power);
                }
                previous_power = rage_power;
                rage_powers.Add(kv.Value, rage_power);
            }

            string rage_extra_use_title_string = "Feature/&BarbarianClassRageExtraUseTitle";
            string rage_extra_use_description_string = "Feature/&BarbarianClassRageExtraUseDescription";

            foreach (var l in rage_uses_increase_levels)
            {
                var feature = Helpers.FeatureBuilder<NewFeatureDefinitions.IncreaseNumberOfPowerUses>.createFeature("BarbarianClassExtraRage" + l.ToString(),
                                                                                                                    "",
                                                                                                                    rage_extra_use_title_string,
                                                                                                                    rage_extra_use_description_string,
                                                                                                                    null,
                                                                                                                    f =>
                                                                                                                    {
                                                                                                                        f.value = 1;
                                                                                                                        f.powers = rage_powers.Where(kv => kv.Key < l).Select(kv => kv.Value)
                                                                                                                                        .Cast<FeatureDefinitionPower>().ToList();
                                                                                                                    }
                                                                                                                    );
                rage_power_extra_use.Add(l, feature);
            }
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
                                                                                                                    a.exclusive = true;
                                                                                                                    a.forbiddenConditions = new List<ConditionDefinition>
                                                                                                                    {
                                                                                                                        DatabaseHelper.ConditionDefinitions.ConditionBarkskin,
                                                                                                                        DatabaseHelper.ConditionDefinitions.ConditionMagicallyArmored
                                                                                                                    };
                                                                                                                }
                                                                                                                );
        }


        static CharacterSubclassDefinition createPathOfFrozenFury()
        {
            createWintersFury();
            createFrigidBody();
            createNumb();

            var gui_presentation = new GuiPresentationBuilder(
                    "Subclass/&BarbarianSubclassPrimalPathOfFrozenFuryDescription",
                    "Subclass/&BarbarianSubclassPrimalPathOfFrozenFuryTitle")
                    .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.DomainElementalCold.GuiPresentation.SpriteReference)
                    .Build();

            CharacterSubclassDefinition definition = new CharacterSubclassDefinitionBuilder("BarbarianSubclassPrimalPathOfFrozenFury", "facf5253-aa04-40b1-89a6-2e09f812da5a")
                    .SetGuiPresentation(gui_presentation)
                    .AddFeatureAtLevel(frozen_fury, 3)
                    .AddFeatureAtLevel(frigid_body, 6)
                    .AddFeatureAtLevel(numb, 10)
                    .AddToDB();

            frozen_fury_rage_feature.requiredSubclass = definition;
            return definition;
        }


        static void createNumb()
        {
            string numb_title_string = "Feature/&BarbarianSubclassFrozenFuryNumbTitle";
            string numb_description_string = "Feature/&BarbarianSubclassFrozenFuryNumbDescription";

            var conditons = new List<ConditionDefinition> { DatabaseHelper.ConditionDefinitions.ConditionPoisoned, DatabaseHelper.ConditionDefinitions.ConditionFrightened };
            var numb_immunity = Helpers.FeatureBuilder<NewFeatureDefinitions.ImmunityToCondtionIfHasSpecificConditions>.createFeature("BarbarianSubclassFrozenFuryNumbImmunity",
                                                                                                                          "",
                                                                                                                          numb_title_string,
                                                                                                                          numb_description_string,
                                                                                                                          null,
                                                                                                                          f =>
                                                                                                                          {
                                                                                                                              f.immuneCondtions = conditons;
                                                                                                                              f.requiredConditions = new List<ConditionDefinition>();
                                                                                                                          }
                                                                                                                          );

            var numb_removal = Helpers.FeatureBuilder<NewFeatureDefinitions.RemoveConditionsOnConditionApplication>.createFeature("BarbarianSubclassFrozenFuryNumbRemoval",
                                                                                                              "",
                                                                                                              numb_title_string,
                                                                                                              numb_description_string,
                                                                                                              null,
                                                                                                              f =>
                                                                                                              {
                                                                                                                  f.removeConditions = conditons;
                                                                                                                  f.appliedConditions = new List<ConditionDefinition>();
                                                                                                              }
                                                                                                              );

            foreach (var rp in rage_powers)
            {
                numb_immunity.requiredConditions.Add(rp.Value.EffectDescription.EffectForms[0].ConditionForm.ConditionDefinition);
                numb_removal.appliedConditions.Add(rp.Value.EffectDescription.EffectForms[0].ConditionForm.ConditionDefinition);
            }


            numb = Helpers.FeatureSetBuilder.createFeatureSet("BarbarianSubclassFrozenFuryNumb",
                                                              "",
                                                              numb_title_string,
                                                              numb_description_string,
                                                              false,
                                                              FeatureDefinitionFeatureSet.FeatureSetMode.Union,
                                                              false,
                                                              numb_immunity,
                                                              numb_removal
                                                              );
        }


        static void createFrigidBody()
        {
            string frigid_body_title_string = "Feature/&BarbarianSubclassFrozenFuryFrigidBodyTitle";
            string frigid_body_description_string = "Feature/&BarbarianSubclassFrozenFuryFrigidBodyDescription";

            frigid_body = Helpers.FeatureBuilder<NewFeatureDefinitions.ArmorBonusAgainstAttackType>.createFeature("BarbarianSubclassFrozenFuryFrigidBody",
                                                                                                                  "",
                                                                                                                  frigid_body_title_string,
                                                                                                                  frigid_body_description_string,
                                                                                                                  null,
                                                                                                                  f =>
                                                                                                                  {
                                                                                                                      f.applyToMelee = false;
                                                                                                                      f.applyToRanged = true;
                                                                                                                      f.requiredConditions = new List<ConditionDefinition>();
                                                                                                                      f.value = 2;
                                                                                                                  }
                                                                                                                  );
            foreach (var rp in rage_powers)
            {
                frigid_body.requiredConditions.Add(rp.Value.EffectDescription.EffectForms[0].ConditionForm.ConditionDefinition);
            }
                                                                
        }


        static void createWintersFury()
        {
            string winters_fury_title_string = "Feature/&BarbarianSubclassFrozenFuryWintersFuryTitle";
            string winters_fury_description_string = "Feature/&BarbarianSubclassFrozenFuryWintersFuryDescription";

            List<(int level, int dice_number, RuleDefinitions.DieType die_type)> frozen_fury_damages = new List<(int level, int dice_number, RuleDefinitions.DieType die_type)>
            {
                {(5, 1, RuleDefinitions.DieType.D6) },
                {(9, 1, RuleDefinitions.DieType.D10) },
                {(13, 2, RuleDefinitions.DieType.D6) },
                {(20, 2, RuleDefinitions.DieType.D10) }
            };

            List<(int, FeatureDefinitionPower)> power_list = new List<(int, FeatureDefinitionPower)>();
            foreach (var entry in frozen_fury_damages)
            {
                var damage = new DamageForm();
                damage.DiceNumber = entry.dice_number;
                damage.DieType = entry.die_type;
                damage.VersatileDieType = entry.die_type;
                damage.DamageType = Helpers.DamageTypes.Cold;

                var effect = new EffectDescription();
                effect.Copy(DatabaseHelper.SpellDefinitions.FireShieldCold.EffectDescription);
                effect.SetRangeType(RuleDefinitions.RangeType.Self);
                effect.SetTargetType(RuleDefinitions.TargetType.Sphere);
                effect.SetTargetSide(RuleDefinitions.Side.All);
                effect.SetTargetParameter(2);
                effect.SetTargetParameter2(1);
                effect.SetRangeParameter(1);
                effect.SetCanBePlacedOnCharacter(true);
                effect.DurationType = RuleDefinitions.DurationType.Instantaneous;
                effect.DurationParameter = 0;
                effect.SetEffectParticleParameters(DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalHeraldOfTheElementsCold.EffectDescription.EffectParticleParameters);
                effect.SetTargetExcludeCaster(true);

                effect.EffectForms.Clear();
                var effect_form = new EffectForm();
                effect_form.DamageForm = damage;
                effect_form.FormType = EffectForm.EffectFormType.Damage;
                effect.EffectForms.Add(effect_form);


                var power = Helpers.PowerBuilder.createPower("BarbarianSubclassFrozenFuryWintersFuryPower" + entry.level,
                                                         "",
                                                         winters_fury_title_string,
                                                         winters_fury_description_string,
                                                         null,
                                                         DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalHeraldOfTheElementsCold,
                                                         effect,
                                                         RuleDefinitions.ActivationTime.NoCost,
                                                         1,
                                                         RuleDefinitions.UsesDetermination.Fixed,
                                                         RuleDefinitions.RechargeRate.AtWill,
                                                         show_casting: false);
                power_list.Add((entry.level, power));
            }

            frozen_fury_rage_feature = Helpers.FeatureBuilder<NewFeatureDefinitions.ApplyPowerOnTurnEndBasedOnClassLevel>.createFeature("BarbarianSubclassFrozenFuryWintersFuryRageFeature",
                                                                                                                                          "",
                                                                                                                                          winters_fury_title_string,
                                                                                                                                          winters_fury_description_string,
                                                                                                                                          null,
                                                                                                                                          f =>
                                                                                                                                          {
                                                                                                                                              f.characterClass = barbarian_class;
                                                                                                                                              f.powerLevelList = power_list;
                                                                                                                                              //will fill subclass in FrozenFuryPath creation
                                                                                                                                          }
                                                                                                                                          );

            foreach (var rp in rage_powers)
            {
                var rage_conditon = rp.Value.EffectDescription.EffectForms[0].conditionForm.conditionDefinition;
                rage_conditon.Features.Add(frozen_fury_rage_feature);
            }

            frozen_fury = Helpers.OnlyDescriptionFeatureBuilder.createOnlyDescriptionFeature("BarbarianSubclassFrozenFuryWintersFuryFeature",
                                                                                             "",
                                                                                             winters_fury_title_string,
                                                                                             winters_fury_description_string
                                                                                             );

        }

        static void createWarShamanSpellcasting()
        {
            war_shaman_spelllist = Helpers.SpelllistBuilder.create9LevelSpelllist("BarbarianSubclassWarshamanSpelllist", "", "",
                                                                    new List<SpellDefinition>
                                                                    {
                                                                                    DatabaseHelper.SpellDefinitions.AnnoyingBee,
                                                                                    DatabaseHelper.SpellDefinitions.Guidance,
                                                                                    DatabaseHelper.SpellDefinitions.PoisonSpray,
                                                                                    DatabaseHelper.SpellDefinitions.Resistance,
                                                                                    DatabaseHelper.SpellDefinitions.Shine,
                                                                                    DatabaseHelper.SpellDefinitions.Sparkle
                                                                    },
                                                                    new List<SpellDefinition>
                                                                    {
                                                                                    DatabaseHelper.SpellDefinitions.AnimalFriendship,
                                                                                    DatabaseHelper.SpellDefinitions.CharmPerson,
                                                                                    DatabaseHelper.SpellDefinitions.CureWounds,
                                                                                    DatabaseHelper.SpellDefinitions.DetectMagic,
                                                                                    DatabaseHelper.SpellDefinitions.Entangle,
                                                                                    DatabaseHelper.SpellDefinitions.FaerieFire,
                                                                                    DatabaseHelper.SpellDefinitions.FogCloud,
                                                                                    DatabaseHelper.SpellDefinitions.Goodberry,
                                                                                    DatabaseHelper.SpellDefinitions.HealingWord,
                                                                                    DatabaseHelper.SpellDefinitions.Jump,
                                                                                    DatabaseHelper.SpellDefinitions.Longstrider,
                                                                                    DatabaseHelper.SpellDefinitions.ProtectionFromEvilGood,
                                                                                    DatabaseHelper.SpellDefinitions.Thunderwave
                                                                    },
                                                                    new List<SpellDefinition>
                                                                    {
                                                                                    DatabaseHelper.SpellDefinitions.Barkskin,
                                                                                    DatabaseHelper.SpellDefinitions.Darkvision,
                                                                                    DatabaseHelper.SpellDefinitions.EnhanceAbility,
                                                                                    DatabaseHelper.SpellDefinitions.FindTraps,
                                                                                    //DatabaseHelper.SpellDefinitions.FlameBlade,
                                                                                    DatabaseHelper.SpellDefinitions.FlamingSphere,
                                                                                    DatabaseHelper.SpellDefinitions.GustOfWind,
                                                                                    DatabaseHelper.SpellDefinitions.HoldPerson,
                                                                                    DatabaseHelper.SpellDefinitions.LesserRestoration,
                                                                                    DatabaseHelper.SpellDefinitions.PassWithoutTrace,
                                                                                    DatabaseHelper.SpellDefinitions.ProtectionFromPoison
                                                                    },
                                                                    new List<SpellDefinition>
                                                                    {
                                                                                    DatabaseHelper.SpellDefinitions.ConjureAnimals,
                                                                                    DatabaseHelper.SpellDefinitions.Daylight,
                                                                                    DatabaseHelper.SpellDefinitions.DispelMagic,
                                                                                    DatabaseHelper.SpellDefinitions.ProtectionFromEnergy,
                                                                                    DatabaseHelper.SpellDefinitions.Revivify,
                                                                                    DatabaseHelper.SpellDefinitions.SleetStorm,
                                                                                    DatabaseHelper.SpellDefinitions.WindWall
                                                                    },
                                                                    new List<SpellDefinition>
                                                                    {
                                                                                    DatabaseHelper.SpellDefinitions.Blight,
                                                                                    DatabaseHelper.SpellDefinitions.Confusion,
                                                                                    DatabaseHelper.SpellDefinitions.ConjureMinorElementals,
                                                                                    DatabaseHelper.SpellDefinitions.DominateBeast,
                                                                                    DatabaseHelper.SpellDefinitions.FireShield,
                                                                                    DatabaseHelper.SpellDefinitions.FreedomOfMovement,
                                                                                    DatabaseHelper.SpellDefinitions.GiantInsect,
                                                                                    DatabaseHelper.SpellDefinitions.IceStorm,
                                                                                    DatabaseHelper.SpellDefinitions.Stoneskin,
                                                                                    DatabaseHelper.SpellDefinitions.WallOfFire
                                                                    }
                                                                    );

            var new_spells = new SpellDefinition[]{ NewFeatureDefinitions.SpellData.getSpell("ShillelaghSpell"),
                                                    NewFeatureDefinitions.SpellData.getSpell("AirBlastSpell"),
                                                    NewFeatureDefinitions.SpellData.getSpell("AcidClawsSpell"),
                                                    NewFeatureDefinitions.SpellData.getSpell("ThunderStrikeSpell"),
                                                    NewFeatureDefinitions.SpellData.getSpell("IceStrikeSpell"),
                                                    NewFeatureDefinitions.SpellData.getSpell("ConjureSpiritAnimalSpell"),
                                                    NewFeatureDefinitions.SpellData.getSpell("FlameBladeSpell"),
                                                    NewFeatureDefinitions.SpellData.getSpell("HeatMetalSpell"),
                                                    NewFeatureDefinitions.SpellData.getSpell("SpikeGrowthSpell"),
                                                    NewFeatureDefinitions.SpellData.getSpell("CallLightningSpell"),
                                                    NewFeatureDefinitions.SpellData.getSpell("WinterBlastSpell"),
                                                  };
            foreach (var s in new_spells)
            {
                if (s != null)
                {
                    Helpers.Misc.addSpellToSpelllist(war_shaman_spelllist, s);
                }
            }

            war_shaman_spelllist.SetMaxSpellLevel(4);
            war_shaman_spelllist.SetHasCantrips(true);

            war_shaman_spellcasting = Helpers.SpellcastingBuilder.createSpontaneousSpellcasting("BarbarianSubclassWarshamanSpellcasting",
                                                                                              "",
                                                                                              "Feature/&BarbarianSubclassWarShamanClassSpellcastingTitle",
                                                                                              "Feature/&BarbarianSubclassWarShamanClassSpellcastingDescription",
                                                                                              war_shaman_spelllist,
                                                                                              Helpers.Stats.Wisdom,
                                                                                              new List<int> {0, 0, 2, 2, 2, 2, 2, 2, 2, 3,
                                                                                                             3, 3, 3, 3, 3, 3, 3, 3, 3, 3 },
                                                                                              new List<int> { 0,  0,  3,  4,  4,  4,  5,  6,  6,  7,
                                                                                                              8,  8,  9, 10, 10, 11, 11, 11, 12, 13},
                                                                                              Helpers.Misc.createSpellSlotsByLevel(new List<int> { 0, 0, 0, 0 }, 
                                                                                                                                   new List<int> { 0, 0, 0, 0 },
                                                                                                                                   new List<int> { 2, 0, 0, 0 },//3
                                                                                                                                   new List<int> { 3, 0, 0, 0 },//4
                                                                                                                                   new List<int> { 3, 0, 0, 0 },//5
                                                                                                                                   new List<int> { 3, 0, 0, 0 },//6
                                                                                                                                   new List<int> { 4, 2, 0, 0 },//7
                                                                                                                                   new List<int> { 4, 2, 0, 0 },//8
                                                                                                                                   new List<int> { 4, 2, 0, 0 },//9
                                                                                                                                   new List<int> { 4, 3, 0, 0 },//10
                                                                                                                                   new List<int> { 4, 3, 0, 0 },//11
                                                                                                                                   new List<int> { 4, 3, 0, 0 },//12
                                                                                                                                   new List<int> { 4, 3, 2, 0 },//13
                                                                                                                                   new List<int> { 4, 3, 2, 0 },//14
                                                                                                                                   new List<int> { 4, 3, 2, 0 },//15
                                                                                                                                   new List<int> { 4, 3, 3, 0 },//16
                                                                                                                                   new List<int> { 4, 3, 3, 0 },//17
                                                                                                                                   new List<int> { 4, 3, 3, 0 },//18
                                                                                                                                   new List<int> { 4, 3, 3, 1 },//19
                                                                                                                                   new List<int> { 4, 3, 3, 1 }//20
                                                                                                                                   )
                                                                                              );
            war_shaman_spellcasting.SetSpellCastingLevel(-1);
            war_shaman_spellcasting.SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Subclass);
            war_shaman_spellcasting.replacedSpells = new List<int> {0, 0, 0, 1, 1, 1, 1, 1, 1, 1,
                                                                    1, 1, 1, 1, 1, 1, 1, 1, 1, 1};
            rage_spellcasting_forbiden.concentrationExceptionFeatures.Add(war_shaman_spellcasting);
        }


        static void createRagecaster()
        {
            string ragecaster_title_string = "Feature/&BarbarianSubclassWarShamanClassRagecasterTitle";
            string ragecaster_description_string = "Feature/&BarbarianSubclassWarShamanClassRagecasterDescription";

            ragecaster = Helpers.OnlyDescriptionFeatureBuilder.createOnlyDescriptionFeature("BarbarianSubclassWarshamanRagecaster",
                                                                                            "",
                                                                                            ragecaster_title_string,
                                                                                            ragecaster_description_string
                                                                                            );
            rage_spellcasting_forbiden.spellcastingExceptionFeatures.Add(ragecaster);
        }


        static void createShareRage()
        {
            string share_rage_title_string = "Feature/&BarbarianSubclassWarShamanClassShareRageTitle";
            string share_rage_description_string = "Feature/&BarbarianSubclassWarShamanClassShareRageDescription";

            NewFeatureDefinitions.PowerWithRestrictions previous_power = null;

            foreach (var kv in rage_bonus_damage_level_map)
            {
                var power = Helpers.CopyFeatureBuilder<NewFeatureDefinitions.PowerWithRestrictions>.createFeatureCopy("BarbarianSubclassWarshamanShareRagePower" + kv.Value.ToString(),
                                                                                                                      "",
                                                                                                                      Helpers.StringProcessing.appendToString(share_rage_title_string,
                                                                                                                                                              share_rage_title_string + kv.Value.ToString(),
                                                                                                                                                              $" (+{kv.Key})"),
                                                                                                                      share_rage_description_string,
                                                                                                                      DatabaseHelper.FeatureDefinitionPowers.PowerDomainLawHolyRetribution.GuiPresentation.SpriteReference,
                                                                                                                      rage_powers[kv.Value]
                                                                                                                      );
                var effect = new EffectDescription();
                effect.Copy(DatabaseHelper.FeatureDefinitionPowers.PowerDomainBattleHeraldOfBattle.EffectDescription);
                effect.SetRangeType(RuleDefinitions.RangeType.Distance);
                effect.SetRangeParameter(6);
                effect.SetTargetParameter(1);
                effect.SetTargetParameter2(1);
                effect.DurationParameter = 1;
                effect.DurationType = RuleDefinitions.DurationType.Minute;
                effect.EffectForms.Clear();
                effect.SetTargetType(RuleDefinitions.TargetType.Individuals);
                effect.SetTargetSide(RuleDefinitions.Side.Ally);
                effect.SetTargetFilteringTag((RuleDefinitions.TargetFilteringTag)(ExtendedEnums.ExtraTargetFilteringTag.NonCaster | ExtendedEnums.ExtraTargetFilteringTag.NoHeavyArmor));
                effect.SetRecurrentEffect(RuleDefinitions.RecurrentEffect.No);

                effect.EffectForms.Add(rage_powers[kv.Value].EffectDescription.EffectForms[0]);
                var effect_form = new EffectForm();
                effect_form.ConditionForm = new ConditionForm();
                effect_form.FormType = EffectForm.EffectFormType.Condition;
                effect_form.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
                effect_form.ConditionForm.ConditionDefinition = rage_powers[kv.Value].EffectDescription.EffectForms[0].conditionForm.conditionDefinition;
                effect_form.conditionForm.SetApplyToSelf(true);
                effect.EffectForms.Add(effect_form);

                power.SetEffectDescription(effect);
                power.SetRechargeRate(RuleDefinitions.RechargeRate.SpellSlot);
                power.SetSpellcastingFeature(war_shaman_spellcasting);
                power.SetFixedUsesPerRecharge(10);

                if (previous_power != null)
                {
                    power.SetOverriddenPower(previous_power);
                }
                previous_power = power;
                power.SetShortTitleOverride(share_rage_title_string);
                power.linkedPower = rage_powers[kv.Value];

                share_rage_powers.Add(kv.Value, power);
            }
        }


        static CharacterSubclassDefinition createPathOfWarShaman()
        {
            createWarShamanSpellcasting();
            createShareRage();
            createRagecaster();

            var gui_presentation = new GuiPresentationBuilder(
                    "Subclass/&BarbarianSubclassPrimalPathOfWarShamanDescription",
                    "Subclass/&BarbarianSubclassPrimalPathOfWarShamanTitle")
                    .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.TraditionGreenmage.GuiPresentation.SpriteReference)
                    .Build();

            CharacterSubclassDefinition definition = new CharacterSubclassDefinitionBuilder("BarbarianSubclassPrimalPathOfWarShaman", "a126dff4-f95b-4352-969a-4b28558f2a82")
                    .SetGuiPresentation(gui_presentation)
                    .AddFeatureAtLevel(war_shaman_spellcasting, 3)
                    .AddFeatureAtLevel(share_rage_powers[1], 6)
                    .AddFeatureAtLevel(share_rage_powers[9], 9)
                    .AddFeatureAtLevel(ragecaster, 10)
                    .AddFeatureAtLevel(share_rage_powers[16], 16)
                    .AddToDB();

            return definition;
        }


        static void createFrenzy()
        {
            string frenzy_exhausted_title_string = "Feature/&BarbarianSubclassBerserkerFrenzyExhaustedTitle";
            string frenzy_exhausted_description_string = "Feature/&BarbarianSubclassBerserkerFrenzyExhaustedDescription";
            string frenzy_condition_title_string = "Rules/&BarbarianSubclassBerserkerFrenzyCondition";


            exhausted_after_frenzy_condition = Helpers.ConditionBuilder.createConditionWithInterruptions("BarbarianSubclassBerserkerFrenzyExhasutedCondition",
                                                                                      "",
                                                                                      frenzy_exhausted_title_string,
                                                                                      frenzy_exhausted_description_string,
                                                                                      null,
                                                                                      DatabaseHelper.ConditionDefinitions.ConditionExhausted,
                                                                                      new RuleDefinitions.ConditionInterruption[] { },
                                                                                      DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityBestowCurseStrength,
                                                                                      DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityBestowCurseDexterity,
                                                                                      DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityBestowCurseConstitution,
                                                                                      DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityBestowCurseIntelligence,
                                                                                      DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityBestowCurseWisdom,
                                                                                      DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityBestowCurseCharisma
                                                                                      );
            exhausted_after_frenzy_condition.SetConditionType(RuleDefinitions.ConditionType.Detrimental);
            exhausted_after_frenzy_condition.SetPossessive(false);

            string frenzy_title_string = "Feature/&BarbarianSubclassBerserkerFrenzyTitle";
            string frenzy_description_string = "Feature/&BarbarianSubclassBerserkerFrenzyDescription";
            var feature_attack = Helpers.FeatureBuilder<NewFeatureDefinitions.ExtraMainWeaponAttack>.createFeature("BarbarianSubclassBerserkerFrenzyExtraAttack",
                                                                                                           "",
                                                                                                           "",
                                                                                                           "",
                                                                                                           null,
                                                                                                           a =>
                                                                                                           {
                                                                                                               a.actionType = ActionDefinitions.ActionType.Bonus;
                                                                                                               a.restrictions = new List<NewFeatureDefinitions.IRestriction>
                                                                                                               {
                                                                                                                   new NewFeatureDefinitions.NoRangedWeaponRestriction()
                                                                                                               };
                                                                                                           }
                                                                                                           );

            var condition = Helpers.ConditionBuilder.createConditionWithInterruptions("BarbarianSubclassBerserkerFrenzyCondition",
                                                                          "",
                                                                          frenzy_condition_title_string,
                                                                          frenzy_description_string,
                                                                          null,
                                                                          DatabaseHelper.ConditionDefinitions.ConditionHeraldOfBattle,
                                                                          new RuleDefinitions.ConditionInterruption[] {},
                                                                          feature_attack
                                                                          );

            var frenzy_watcher = Helpers.FeatureBuilder<NewFeatureDefinitions.FrenzyWatcher>.createFeature("BarbarianSubclassBerserkerFrenzyWatcher",
                                                                               "",
                                                                               Common.common_no_title,
                                                                               Common.common_no_title,
                                                                               null,
                                                                               r =>
                                                                               {
                                                                                   r.requiredConditions = rage_powers.Select(kv => kv.Value.EffectDescription.EffectForms[0].conditionForm.conditionDefinition).ToList();
                                                                                   r.targetCondition = condition;
                                                                                   r.afterCondition = exhausted_after_frenzy_condition;
                                                                               }
                                                                               );

            condition.Features.Add(frenzy_watcher);
            condition.SetPossessive(false);

            var effect = new EffectDescription();
            effect.Copy(DatabaseHelper.SpellDefinitions.Haste.EffectDescription);
            effect.SetRangeType(RuleDefinitions.RangeType.Self);
            effect.SetRangeParameter(1);
            effect.DurationParameter = 1;
            effect.DurationType = RuleDefinitions.DurationType.UntilLongRest;
            effect.EffectForms.Clear();
            effect.SetTargetType(RuleDefinitions.TargetType.Self);

            var effect_form = new EffectForm();
            effect_form.ConditionForm = new ConditionForm();
            effect_form.FormType = EffectForm.EffectFormType.Condition;
            effect_form.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            effect_form.ConditionForm.ConditionDefinition = condition;
            effect.EffectForms.Add(effect_form);

            frenzy = Helpers.GenericPowerBuilder<NewFeatureDefinitions.PowerWithRestrictions>
                                                      .createPower("BarbarianSubclassBerserkerFrenzyPower",
                                                         "",
                                                         frenzy_title_string,
                                                         frenzy_description_string,
                                                         DatabaseHelper.FeatureDefinitionPowers.PowerOathOfTirmarSmiteTheHidden.GuiPresentation.SpriteReference,
                                                         effect,
                                                         RuleDefinitions.ActivationTime.NoCost,
                                                         1,
                                                         RuleDefinitions.UsesDetermination.Fixed,
                                                         RuleDefinitions.RechargeRate.ShortRest
                                                         );
            frenzy.restrictions = new List<NewFeatureDefinitions.IRestriction>()
            {
                new NewFeatureDefinitions.InBattleRestriction(),
                new NewFeatureDefinitions.NoConditionRestriction(exhausted_after_frenzy_condition),
                new NewFeatureDefinitions.HasAtLeastOneConditionFromListRestriction(rage_powers.Select(kv => kv.Value.EffectDescription.EffectForms[0].conditionForm.conditionDefinition).ToArray())
            };

            frenzy.SetShortTitleOverride(frenzy_title_string);
        }


        static void createMindlessRage()
        {
            string mindless_rage_title_string = "Feature/&BarbarianSubclassMindlessRageTitle";
            string mindless_rage_description_string = "Feature/&BarbarianSubclassMindlessRageDescription";

            var conditons = new List<ConditionDefinition> { DatabaseHelper.ConditionDefinitions.ConditionCharmed, DatabaseHelper.ConditionDefinitions.ConditionFrightened };
            var immunity = Helpers.FeatureBuilder<NewFeatureDefinitions.ImmunityToCondtionIfHasSpecificConditions>.createFeature("BarbarianSubclassBerserkerMindlessRageImmunity",
                                                                                                                                    "",
                                                                                                                                    mindless_rage_title_string,
                                                                                                                                    mindless_rage_description_string,
                                                                                                                                    null,
                                                                                                                                    f =>
                                                                                                                                    {
                                                                                                                                        f.immuneCondtions = conditons;
                                                                                                                                        f.requiredConditions = new List<ConditionDefinition>();
                                                                                                                                    }
                                                                                                                                    );

            var removal = Helpers.FeatureBuilder<NewFeatureDefinitions.RemoveConditionsOnConditionApplication>.createFeature("BarbarianSubclassBerserkerMindlessRageRemoval",
                                                                                                                              "",
                                                                                                                              mindless_rage_title_string,
                                                                                                                              mindless_rage_description_string,
                                                                                                                              null,
                                                                                                                              f =>
                                                                                                                              {
                                                                                                                                  f.removeConditions = conditons;
                                                                                                                                  f.appliedConditions = new List<ConditionDefinition>();
                                                                                                                              }
                                                                                                                              );

            foreach (var rp in rage_powers)
            {
                immunity.requiredConditions.Add(rp.Value.EffectDescription.EffectForms[0].ConditionForm.ConditionDefinition);
                removal.appliedConditions.Add(rp.Value.EffectDescription.EffectForms[0].ConditionForm.ConditionDefinition);
            }


            mindless_rage = Helpers.FeatureSetBuilder.createFeatureSet("BarbarianSubclassBerserkerMindlessRage",
                                                                          "",
                                                                          mindless_rage_title_string,
                                                                          mindless_rage_description_string,
                                                                          false,
                                                                          FeatureDefinitionFeatureSet.FeatureSetMode.Union,
                                                                          false,
                                                                          immunity,
                                                                          removal
                                                                          );
        }


        static void createIntimidatingPresence()
        {
            string intimidating_presence_title_string = "Feature/&BarbarianSubclassBerserkerIntimidatingPresenceTitle";
            string intimidating_presence_description_string = "Feature/&BarbarianSubclassBerserkerIntimidatingPresenceDescription";

            var immune_condition = Helpers.CopyFeatureBuilder<ConditionDefinition>.createFeatureCopy("BarbarianSubclassBerserkerIntimidatingPresenceImmunityCondition",
                                                                                                     "",
                                                                                                     Common.common_no_title,
                                                                                                     Common.common_no_title,
                                                                                                     Common.common_no_icon,
                                                                                                     DatabaseHelper.ConditionDefinitions.ConditionTemporaryHitPoints,
                                                                                                     c =>
                                                                                                     {
                                                                                                         c.features = new List<FeatureDefinition>();
                                                                                                         c.SetConditionType(RuleDefinitions.ConditionType.Neutral);
                                                                                                         c.SetSpecialDuration(true);
                                                                                                         c.SetDurationType(RuleDefinitions.DurationType.UntilLongRest);
                                                                                                         c.SetDurationParameterDie(RuleDefinitions.DieType.D1);
                                                                                                     }
                                                                                                     );
            immune_condition.SetSilentWhenAdded(true);
            immune_condition.SetSilentWhenRemoved(true);


            //Add to our new effect
            EffectDescription effect_description = new EffectDescription();
            effect_description.Copy(DatabaseHelper.FeatureDefinitionPowers.PowerDomainOblivionMarkOfFate.EffectDescription);
            effect_description.SetSavingThrowDifficultyAbility(Helpers.Stats.Charisma);
            effect_description.SetDifficultyClassComputation(RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency);
            effect_description.SavingThrowAbility = Helpers.Stats.Wisdom;
            effect_description.HasSavingThrow = true;
            effect_description.DurationType = RuleDefinitions.DurationType.Round;
            effect_description.DurationParameter = 1;
            effect_description.SetRangeType(RuleDefinitions.RangeType.Distance);
            effect_description.SetRangeParameter(6);
            effect_description.SetTargetType(RuleDefinitions.TargetType.Individuals);
            effect_description.SetTargetSide(RuleDefinitions.Side.Enemy);
            effect_description.SetEndOfEffect(RuleDefinitions.TurnOccurenceType.EndOfTurn);
            effect_description.immuneCreatureFamilies = new List<string> {Helpers.Misc.createImmuneIfHasConditionFamily(immune_condition) };
            

            effect_description.EffectForms.Clear();
            EffectForm effect_form = new EffectForm();
            effect_form.FormType = EffectForm.EffectFormType.Condition;
            effect_form.ConditionForm = new ConditionForm();
            effect_form.ConditionForm.ConditionDefinition = DatabaseHelper.ConditionDefinitions.ConditionFrightened;
            effect_form.hasSavingThrow = true;
            effect_form.SavingThrowAffinity = RuleDefinitions.EffectSavingThrowType.Negates;
            effect_form.conditionForm.operation = ConditionForm.ConditionOperation.Add;
            effect_description.EffectForms.Add(effect_form);

            intimidating_presence = Helpers.GenericPowerBuilder<FeatureDefinitionPower>
                                                          .createPower("BarbarianSubclassBerserkerIntimidatingPresencePower",
                                                             "",
                                                             intimidating_presence_title_string,
                                                             intimidating_presence_description_string,
                                                             //DatabaseHelper.FeatureDefinitionPowers.PowerDomainOblivionMarkOfFate.GuiPresentation.SpriteReference,
                                                             DatabaseHelper.SpellDefinitions.Fear.GuiPresentation.SpriteReference,
                                                             effect_description,
                                                             RuleDefinitions.ActivationTime.Action,
                                                             1,
                                                             RuleDefinitions.UsesDetermination.Fixed,
                                                             RuleDefinitions.RechargeRate.AtWill
                                                             );
            var immune_application = Helpers.FeatureBuilder<NewFeatureDefinitions.ApplyConditionOnPowerUseToTarget>.createFeature("BarbarianSubclassBerserkerIntimidatingPresenceApplyImmuneFeature",
                                                                                                                                  "",
                                                                                                                                  Common.common_no_title,
                                                                                                                                  Common.common_no_title,
                                                                                                                                  Common.common_no_icon,
                                                                                                                                  a =>
                                                                                                                                  {
                                                                                                                                      a.condition = immune_condition;
                                                                                                                                      a.durationType = RuleDefinitions.DurationType.Day;
                                                                                                                                      a.durationValue = 1;
                                                                                                                                      a.turnOccurence = RuleDefinitions.TurnOccurenceType.EndOfTurn;
                                                                                                                                      a.power = intimidating_presence;
                                                                                                                                      a.onlyOnSucessfulSave = true;
                                                                                                                                  }
                                                                                                                                  );
            intimidating_presence_feature = Helpers.FeatureSetBuilder.createFeatureSet("BarbarianSubclassBerserkerIntimidatingPresenceFeatureSet",
                                                                                       "",
                                                                                       intimidating_presence_title_string,
                                                                                       intimidating_presence_description_string,
                                                                                       false,
                                                                                       FeatureDefinitionFeatureSet.FeatureSetMode.Union,
                                                                                       false,
                                                                                       immune_application,
                                                                                       intimidating_presence
                                                                                       );
        }



        static CharacterSubclassDefinition createPathOfBerserker()
        {
            createFrenzy();
            createMindlessRage();
            createIntimidatingPresence();


            var gui_presentation = new GuiPresentationBuilder(
                    "Subclass/&BarbarianSubclassPrimalPathOfBerserkerDescription",
                    "Subclass/&BarbarianSubclassPrimalPathOfBerserkerTitle")
                    .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.RoguishDarkweaver.GuiPresentation.SpriteReference)
                    .Build();

            CharacterSubclassDefinition definition = new CharacterSubclassDefinitionBuilder("BarbarianSubclassPrimalPathOfBersrker", "3356b777-cf53-469c-93f4-766c52664016")
                    .SetGuiPresentation(gui_presentation)
                    .AddFeatureAtLevel(frenzy, 3)
                    .AddFeatureAtLevel(mindless_rage, 6)
                    .AddFeatureAtLevel(intimidating_presence_feature, 10)
                    .AddToDB();

            return definition;
        }


        public static void BuildAndAddClassToDB()
        {
            var BarbarianClass = new BarbarianClassBuilder(BarbarianClassName, BarbarianClassNameGuid).AddToDB();
            BarbarianClass.FeatureUnlocks.Sort(delegate (FeatureUnlockByLevel a, FeatureUnlockByLevel b)
                                          {
                                              return a.Level - b.Level;
                                          }
                                         );

            BarbarianFeatureDefinitionSubclassChoice.Subclasses.Add(createPathOfBerserker().Name);
            BarbarianFeatureDefinitionSubclassChoice.Subclasses.Add(createPathOfFrozenFury().Name);
            BarbarianFeatureDefinitionSubclassChoice.Subclasses.Add(createPathOfWarShaman().Name);
        }

        private static FeatureDefinitionSubclassChoice BarbarianFeatureDefinitionSubclassChoice;
    }
}
