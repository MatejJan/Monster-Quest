using System.Collections.Generic;
using System.Linq;
using MonsterQuest.Effects;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace MonsterQuest.Editor
{
    [CustomEditor(typeof(MonsterType))]
    public class MonsterTypeEditor : UnityEditor.Editor
    {
        [SerializeField] private VisualTreeAsset layout;

        private readonly Dictionary<RuleCategory, string> _ruleCategoryVisualElementNames = new()
        {
            {
                RuleCategory.SpecialTrait, "special-traits"
            },
            {
                RuleCategory.Action, "actions"
            },
            {
                RuleCategory.Reaction, "reactions"
            },
            {
                RuleCategory.LegendaryAction, "legendary-actions"
            }
        };

        private bool _displayNameAutocompleteHovered;

        private IntegerField _telepathyRangeField;

        private TextField _displayNameField;

        private Toggle _blindToggle;

        private Toggle _languageAbilitiesToggle;

        private Toggle _senseRangesToggle;
        private VisualElement _displayNameAutocomplete;

        private VisualElement _root;

        public override VisualElement CreateInspectorGUI()
        {
            _root = new VisualElement();
            layout.CloneTree(_root);

            _root.TrackSerializedObjectValue(serializedObject, OnSerializedObjectUpdated);
            _root.RegisterCallback<GeometryChangedEvent>(OnRootGeometryChanged);

            _displayNameField = _root.Q<TextField>("display-name");
            _displayNameField.RegisterValueChangedCallback(OnDisplayNameChanged);
            _displayNameField.RegisterCallback<FocusEvent>(OnDisplayNameFocus);
            _displayNameField.RegisterCallback<BlurEvent>(OnDisplayNameBlur);

            _displayNameAutocomplete = _root.Q<VisualElement>("display-name-autocomplete");
            _displayNameAutocomplete.RegisterCallback<MouseEnterEvent>(OnDisplayNameAutocompleteMouseEnter);
            _displayNameAutocomplete.RegisterCallback<MouseLeaveEvent>(OnDisplayNameAutocompleteMouseLeave);

            Button importButton = _root.Q<Button>("import-button");
            importButton.RegisterCallback<ClickEvent>(OnImportButtonClick);

            MonsterTypeImporter.Initialize();

            return _root;
        }

        private void OnDisplayNameChanged(ChangeEvent<string> _)
        {
            UpdateDisplayNameAutocomplete();
        }

        private void OnDisplayNameFocus(FocusEvent _)
        {
            UpdateDisplayNameAutocomplete();
            ShowDisplayNameAutocomplete();
        }

        private void OnDisplayNameBlur(BlurEvent _)
        {
            if (!_displayNameAutocompleteHovered)
            {
                HideDisplayNameAutocomplete();
            }
        }

        private void OnDisplayNameAutocompleteMouseEnter(MouseEnterEvent _)
        {
            _displayNameAutocompleteHovered = true;
        }

        private void OnDisplayNameAutocompleteMouseLeave(MouseLeaveEvent _)
        {
            _displayNameAutocompleteHovered = false;
        }

        private void OnRootGeometryChanged(GeometryChangedEvent _)
        {
            _root.UnregisterCallback<GeometryChangedEvent>(OnRootGeometryChanged);
            InitialRefresh();
        }

        private void OnImportButtonClick(ClickEvent _)
        {
            if (serializedObject.targetObject is not MonsterType monsterType) return;

            MonsterTypeImporter.ImportData(monsterType);
        }

        private void InitialRefresh()
        {
            OnSerializedObjectUpdated(null);

            _blindToggle = _root.Q<Toggle>("blind");
            _senseRangesToggle = _root.Q("sense-ranges").Q<Toggle>();
            _senseRangesToggle.RegisterValueChangedCallback(UpdateBlindVisibility);
            UpdateBlindVisibility(null);

            _telepathyRangeField = _root.Q<IntegerField>("telepathy-range");
            _languageAbilitiesToggle = _root.Q("language-abilities").Q<Toggle>();
            _languageAbilitiesToggle.RegisterValueChangedCallback(UpdateTelepathyRangeVisibility);
            UpdateTelepathyRangeVisibility(null);
        }

        private void UpdateBlindVisibility(ChangeEvent<bool> _)
        {
            if (_senseRangesToggle is null) return;

            _blindToggle.style.display = _senseRangesToggle.value ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void UpdateTelepathyRangeVisibility(ChangeEvent<bool> _)
        {
            if (_languageAbilitiesToggle is null) return;

            _telepathyRangeField.style.display = _languageAbilitiesToggle.value ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void UpdateDisplayNameAutocomplete()
        {
            if (serializedObject.targetObject is not MonsterType monsterType) return;
            if (monsterType.displayName is null) return;

            _displayNameAutocomplete.Clear();

            foreach (string monsterName in MonsterTypeImporter.GetMatchingMonsterNames(monsterType.displayName))
            {
                Label label = new()
                {
                    text = monsterName
                };

                label.AddToClassList("entry");

                label.RegisterCallback<ClickEvent>(clickEvent =>
                {
                    monsterType.displayName = monsterName.ToLower();
                    HideDisplayNameAutocomplete();
                });

                _displayNameAutocomplete.Add(label);
            }
        }

        private void ShowDisplayNameAutocomplete()
        {
            _displayNameAutocomplete.style.display = DisplayStyle.Flex;
        }

        private void HideDisplayNameAutocomplete()
        {
            _displayNameAutocomplete.style.display = DisplayStyle.None;
        }

        private void OnSerializedObjectUpdated(SerializedObject _)
        {
            if (serializedObject.targetObject is not MonsterType monsterType) return;

            UpdateSpeed(monsterType);
            UpdateSavingThrowBonuses(monsterType);
            UpdateSkillBonuses(monsterType);
            UpdateDamageVulnerabilities(monsterType);
            UpdateDamageResistances(monsterType);
            UpdateDamageImmunities(monsterType);
            UpdateConditionImmunities(monsterType);
            UpdateSenseRanges(monsterType);
            UpdateLanguageAbilities(monsterType);
            UpdateExperiencePoints(monsterType);
            UpdateProficiencyBonus(monsterType);
            UpdateRules(monsterType);
            UpdatePreview(monsterType);
        }

        private void UpdateSpeed(MonsterType monsterType)
        {
            string description = $"{monsterType.speed.walk} ft.";

            if (monsterType.speed.burrow > 0)
            {
                description += $", burrow {monsterType.speed.burrow} ft.";
            }

            if (monsterType.speed.climb > 0)
            {
                description += $", climb {monsterType.speed.climb} ft.";
            }

            if (monsterType.speed.fly > 0)
            {
                description += $", fly {monsterType.speed.fly} ft.";
            }

            if (monsterType.speed.hover)
            {
                description += " (hover)";
            }

            if (monsterType.speed.swim > 0)
            {
                description += $", swim {monsterType.speed.swim} ft.";
            }

            UpdateFoldoutLabel("speed", string.Join(", ", description));
        }

        private void UpdateSavingThrowBonuses(MonsterType monsterType)
        {
            if (monsterType.savingThrowBonuses is null) return;

            IEnumerable<string> descriptions = monsterType.savingThrowBonuses.Select(bonus => $"{bonus.ability.ToString()[..3]} {bonus.amount:+#;-#;+0}");
            UpdateFoldoutLabel("saving-throw-bonuses", string.Join(", ", descriptions));
        }

        private void UpdateSkillBonuses(MonsterType monsterType)
        {
            if (monsterType.skillBonuses is null) return;

            IEnumerable<string> descriptions = monsterType.skillBonuses.Select(bonus => $"{bonus.skill} {bonus.amount:+#;-#;+0}");
            UpdateFoldoutLabel("skill-bonuses", string.Join(", ", descriptions));
        }

        private void UpdateDamageVulnerabilities(MonsterType monsterType)
        {
            if (monsterType.damageVulnerabilities is null) return;

            UpdateFoldoutLabel("damage-vulnerabilities", GetDamageTypeDescription(monsterType.damageVulnerabilities));
        }

        private void UpdateDamageResistances(MonsterType monsterType)
        {
            if (monsterType.damageResistances is null) return;

            UpdateFoldoutLabel("damage-resistances", GetDamageTypeDescription(monsterType.damageResistances));
        }

        private void UpdateDamageImmunities(MonsterType monsterType)
        {
            if (monsterType.damageImmunities is null) return;

            UpdateFoldoutLabel("damage-immunities", GetDamageTypeDescription(monsterType.damageImmunities));
        }

        private string GetDamageTypeDescription(DamageType[] damageTypes)
        {
            DamageType[] normalDamageTypes = damageTypes.Where(damageType => (damageType & DamageType.Nonmagical) != DamageType.Nonmagical && (damageType & DamageType.Magical) != DamageType.Magical).ToArray();
            DamageType[] nonMagicalDamageTypes = damageTypes.Where(damageType => (damageType & DamageType.Nonmagical) == DamageType.Nonmagical).Select(damageType => damageType & ~DamageType.Nonmagical).ToArray();
            DamageType[] magicalDamageTypes = damageTypes.Where(damageType => (damageType & DamageType.Magical) == DamageType.Magical).Select(damageType => damageType & ~DamageType.Magical).ToArray();

            List<string> descriptionParts = new();
            if (normalDamageTypes.Length > 0) descriptionParts.Add(string.Join(", ", normalDamageTypes).ToLower());
            if (nonMagicalDamageTypes.Length > 0) descriptionParts.Add($"{string.Join(", ", nonMagicalDamageTypes).ToLower()} from nonmagical attacks");
            if (magicalDamageTypes.Length > 0) descriptionParts.Add($"{string.Join(", ", magicalDamageTypes).ToLower()} from magical attacks");

            return string.Join("; ", descriptionParts);
        }

        private void UpdateConditionImmunities(MonsterType monsterType)
        {
            if (monsterType.conditionImmunities is null) return;

            IEnumerable<string> descriptions = monsterType.conditionImmunities.Select(damageType => damageType.ToString().ToLower());
            UpdateFoldoutLabel("condition-immunities", string.Join(", ", descriptions));
        }

        private void UpdateSenseRanges(MonsterType monsterType)
        {
            if (monsterType.senseRanges is null) return;

            List<string> descriptions = monsterType.senseRanges.Select(senseRange => $"{senseRange.sense.ToString().ToLower()} {senseRange.range} ft.").ToList();

            if (monsterType.blind)
            {
                int blindsightIndex = descriptions.FindIndex(description => description.Contains("blindsight"));

                if (blindsightIndex > -1)
                {
                    descriptions[blindsightIndex] = $"{descriptions[blindsightIndex]} (blind beyond this radius)";
                }
            }

            descriptions.Add($"passive Perception {monsterType.passivePerception}");
            UpdateFoldoutLabel("sense-ranges", string.Join(", ", descriptions));
        }

        private void UpdateLanguageAbilities(MonsterType monsterType)
        {
            if (monsterType.languageAbilities is null) return;

            List<string> descriptions = monsterType.languageAbilities.Where(ability => ability.canSpeak).Select(ability => ability.language.ToString()).ToList();
            Language[] understoodLanguages = monsterType.languageAbilities.Where(ability => !ability.canSpeak).Select(ability => ability.language).ToArray();

            if (understoodLanguages.Length > 0)
            {
                string understoodDescription = $"understands {EnglishHelper.JoinWithAnd(understoodLanguages)} but can't speak";

                if (descriptions.Count > 0)
                {
                    understoodDescription += $" {(understoodLanguages.Length == 1 ? "it" : "them")}";
                }

                descriptions.Add(understoodDescription);
            }

            if (monsterType.telepathyRange > 0)
            {
                descriptions.Add($"telepathy {monsterType.telepathyRange} ft.");
            }

            if (descriptions.Count == 0) descriptions.Add("—");

            UpdateFoldoutLabel("language-abilities", string.Join(", ", descriptions));
        }

        private void UpdateExperiencePoints(MonsterType monsterType)
        {
            Label experiencePointsLabel = _root.Q<Label>("experience-points");
            experiencePointsLabel.text = $" ({monsterType.experiencePoints} XP)";
        }

        private void UpdateProficiencyBonus(MonsterType monsterType)
        {
            Label experiencePointsLabel = _root.Q<Label>("proficiency-bonus");
            experiencePointsLabel.text = $"{monsterType.proficiencyBonus:+#;-#;+0}";
        }

        private void UpdateFoldoutLabel(string propertyFieldName, string extraText)
        {
            PropertyField propertyField = _root.Q<PropertyField>(propertyFieldName);
            Label label = propertyField.Q<Label>(className: "unity-foldout__text");

            if (label is null) return;

            label.text = $"<b>{propertyField.label}</b> {extraText}";
        }

        private void UpdateRules(MonsterType monsterType)
        {
            if (monsterType.items is null) return;

            List<RuleDescription> ruleDescriptions = new();

            foreach (ItemType item in monsterType.items)
            {
                if (item is null) continue;

                object context = null;

                if (item.HasEffect<WeaponType>())
                {
                    context = new InformativeMonsterAttackAction(monsterType, null, item);
                }

                ruleDescriptions.AddRange(item.GetOwnRuleDescriptions(context));
            }

            foreach (EffectType effect in monsterType.effects)
            {
                if (effect is null) continue;

                object context = null;

                if (effect is AttackType attack)
                {
                    context = new InformativeMonsterAttackAction(monsterType, attack, null);
                }

                ruleDescriptions.AddRange(effect.GetOwnRuleDescriptions(context));
            }

            foreach (KeyValuePair<RuleCategory, string> ruleCategoryVisualElementNamesEntry in _ruleCategoryVisualElementNames)
            {
                VisualElement parent = _root.Q(ruleCategoryVisualElementNamesEntry.Value);
                parent.Clear();

                foreach (RuleDescription ruleDescription in ruleDescriptions.Where(ruleDescription => ruleDescription.category == ruleCategoryVisualElementNamesEntry.Key))
                {
                    AddRuleDescription(parent, ruleDescription);
                }
            }
        }

        private void AddRuleDescription(VisualElement parent, RuleDescription ruleDescription)
        {
            string type = ruleDescription.type is null ? "" : $" {ruleDescription.type.ToStartCase()}:";

            Label label = new()
            {
                text = $"<i><b>{ruleDescription.name.ToStartCase()}.</b>{type}</i> {ruleDescription.description}"
            };

            label.AddToClassList("rule");
            parent.Add(label);
        }

        private void UpdatePreview(MonsterType monsterType)
        {
            Image preview = _root.Q<Image>("preview");
            preview.sprite = monsterType.bodySprite;
        }
    }
}
