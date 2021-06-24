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
        static public Dictionary<int, NewFeatureDefinitions.PowerWithRestrictions> rage_powers = new Dictionary<int, NewFeatureDefinitions.PowerWithRestrictions>();
        static public NewFeatureDefinitions.PowerWithRestrictions reckless_attack_power;
        static public FeatureDefinitionFeatureSet reckless_attack;
        static public NewFeatureDefinitions.SavingthrowAffinityUnderRestriction danger_sense;
        static public Dictionary<int, NewFeatureDefinitions.IncreaseNumberOfPowerUses> rage_power_extra_use = new Dictionary<int, NewFeatureDefinitions.IncreaseNumberOfPowerUses>();
        static public FeatureDefinitionAttributeModifier extra_attack;
        static public FeatureDefinitionMovementAffinity fast_movement;
        static public FeatureDefinitionFeatureSet feral_instinct;
        static public NewFeatureDefinitions.WeaponDamageDiceIncreaseOnCriticalHit brutal_critical;
        //Paths: Berserker, War shaman, frozen fury
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
            subclassChoicesGuiPresentation.Title = "Subclass/&BarbarianSubclassCollegeTitle";
            subclassChoicesGuiPresentation.Description = "Subclass/&BarbarianSubclassCollegeDescription";
            BarbarianFeatureDefinitionSubclassChoice = this.BuildSubclassChoice(3, "College", false, "SubclassChoiceBarbarianSpecialistArchetypes", subclassChoicesGuiPresentation, BarbarianClassSubclassesGuid);
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

            fast_movement = Helpers.CopyFeatureBuilder<FeatureDefinitionMovementAffinity>.createFeatureCopy("BarbarianClassFastMovement",
                                                                                                            "",
                                                                                                            fast_movement_title_string,
                                                                                                            fast_movement_description_string,
                                                                                                            null,
                                                                                                            DatabaseHelper.FeatureDefinitionMovementAffinitys.MovementAffinityLongstrider
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
            string reckless_attack_condition_string = "Rules/&BarbarianClassRecklessAttackCondition";

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
                                                                                      reckless_attack_title_string,
                                                                                      reckless_attack_description_string,
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

            var reckless_attack_watcher = Helpers.FeatureBuilder<NewFeatureDefinitions.ApplyConditionOnAttackToAttackerUnitUntilTurnStart>.createFeature("BarbarianClassAttackedMark",
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
                                                         DatabaseHelper.FeatureDefinitionPowers.PowerDomainBattleDivineWrath.GuiPresentation.SpriteReference,
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
                                                                                          new RuleDefinitions.ConditionInterruption[] { },
                                                                                          DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityBludgeoningResistance,
                                                                                          DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPiercingResistance,
                                                                                          DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinitySlashingResistance,
                                                                                          DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionBullsStrength,
                                                                                          damage_bonus
                                                                                          );

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
                                                             DatabaseHelper.FeatureDefinitionPowers.PowerDomainBattleHeraldOfBattle.GuiPresentation.SpriteReference,
                                                             effect,
                                                             RuleDefinitions.ActivationTime.BonusAction,
                                                             2 + rage_uses_increase_levels.Count(l => l < kv.Value),
                                                             RuleDefinitions.UsesDetermination.Fixed,
                                                             RuleDefinitions.RechargeRate.LongRest
                                                             );
                rage_power.restrictions = new List<NewFeatureDefinitions.IRestriction>()
                {
                    new NewFeatureDefinitions.InBattleRestriction(),
                    new NewFeatureDefinitions.NoConditionRestriction(condition_can_continue_rage)
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
