using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

using ACE.DatLoader.Entity;
using ACE.DatLoader.Entity.AnimationHooks;
using ACE.Entity.Enum;

using AttackFrameParams = ACE.Entity.AttackFrameParams;

namespace ACE.DatLoader.FileTypes
{
    [DatFileType(DatFileType.MotionTable)]
    public class MotionTable : FileType
    {
        public uint DefaultStyle { get; private set; }
        public Dictionary<uint, uint> StyleDefaults { get; } = new Dictionary<uint, uint>();
        public Dictionary<uint, MotionData> Cycles { get; } = new Dictionary<uint, MotionData>();
        public Dictionary<uint, MotionData> Modifiers { get; } = new Dictionary<uint, MotionData>();
        public Dictionary<uint, Dictionary<uint, MotionData>> Links { get; } = new Dictionary<uint, Dictionary<uint, MotionData>>();

        public override void Unpack(BinaryReader reader)
        {
            Id = reader.ReadUInt32();

            DefaultStyle = reader.ReadUInt32();

            uint numStyleDefaults = reader.ReadUInt32();
            for (uint i = 0; i < numStyleDefaults; i++)
                StyleDefaults.Add(reader.ReadUInt32(), reader.ReadUInt32());

            Cycles.Unpack(reader);

            Modifiers.Unpack(reader);

            Links.Unpack(reader);
        }

        /// <summary>
        /// Gets the default style for the requested MotionStance
        /// </summary>
        /// <returns>The default style or MotionCommand.Invalid if not found</returns>
        private MotionCommand GetDefaultMotion(MotionStance style)
        {
            if (StyleDefaults.ContainsKey((uint)style))
                return (MotionCommand)StyleDefaults[(uint)style];

            return MotionCommand.Invalid;
        }

        public float GetAnimationLength(MotionCommand motion)
        {
            var defaultStance = (MotionStance)DefaultStyle;
            var defaultMotion = GetDefaultMotion(defaultStance);

            return GetAnimationLength(defaultStance, motion, defaultMotion);
        }

        public float GetAnimationLength(MotionStance stance, MotionCommand motion, MotionCommand? currentMotion = null)
        {
            if (currentMotion == null)
                currentMotion = GetDefaultMotion(stance);

            return GetAnimationLength(stance, motion, currentMotion.Value);
        }

        public float GetCycleLength(MotionStance stance, MotionCommand motion)
        {
            uint key = (uint)stance << 16 | (uint)motion & 0xFFFFF;

            Cycles.TryGetValue(key, out var motionData);

            if (motionData == null)
                return 0.0f;

            var length = 0.0f;
            foreach (var anim in motionData.Anims)
                length += GetAnimationLength(anim);

            return length;
        }

        private static readonly ConcurrentDictionary<AttackFrameParams, List<(float time, AttackHook attackHook)>> attackFrameCache = new ConcurrentDictionary<AttackFrameParams, List<(float time, AttackHook attackHook)>>();

        public List<(float time, AttackHook attackHook)> GetAttackFrames(uint motionTableId, MotionStance stance, MotionCommand motion)
        {
            // could also do uint, and then a packed ulong, but would be more complicated maybe?
            var attackFrameParams = new AttackFrameParams(motionTableId, stance, motion);
            if (attackFrameCache.TryGetValue(attackFrameParams, out var attackFrames))
                return attackFrames;

            var motionTable = DatManager.PortalDat.ReadFromDat<MotionTable>(motionTableId);

            var defaultMotion = GetDefaultMotion(stance);

            var animData = GetAnimData(stance, motion, defaultMotion);

            var frameNums = new List<int>();
            var attackHooks = new List<AttackHook>();
            var totalFrames = 0;

            foreach (var anim in animData)
            {
                var animation = DatManager.PortalDat.ReadFromDat<Animation>(anim.AnimId);

                foreach (var frame in animation.PartFrames)
                {
                    foreach (var hook in frame.Hooks)
                    {
                        if (hook is AttackHook attackHook)
                        {
                            frameNums.Add(totalFrames);
                            attackHooks.Add(attackHook);
                        }
                    }
                    totalFrames++;
                }
            }
            attackFrames = new List<(float time, AttackHook attackHook)>();
            for (var i = 0; i < frameNums.Count; i++)
                attackFrames.Add(((float)frameNums[i] / totalFrames, attackHooks[i]));    // div 0?

            attackFrameCache.TryAdd(attackFrameParams, attackFrames);

            return attackFrames;
        }

        public List<AnimData> GetAnimData(MotionStance stance, MotionCommand motion, MotionCommand currentMotion)
        {
            var animData = new List<AnimData>();

            uint motionHash = (uint)stance << 16 | (uint)currentMotion & 0xFFFFF;

            Links.TryGetValue(motionHash, out var link);
            if (link == null) return animData;

            link.TryGetValue((uint)motion, out var motionData);
            if (motionData == null)
            {
                motionHash = (uint)stance << 16;
                Links.TryGetValue(motionHash, out link);
                if (link == null) return animData;
                link.TryGetValue((uint)motion, out motionData);
                if (motionData == null) return animData;
            }
            return motionData.Anims;
        }

        public float GetAnimationLength(MotionStance stance, MotionCommand motion, MotionCommand currentMotion)
        {
            var animData = GetAnimData(stance, motion, currentMotion);

            var length = 0.0f;
            foreach (var anim in animData)
                length += GetAnimationLength(anim);

            return length;
        }

        public float GetAnimationLength(AnimData anim)
        {
            var highFrame = anim.HighFrame;

            // get the maximum # of animation frames
            var animation = DatManager.PortalDat.ReadFromDat<Animation>(anim.AnimId);

            if (anim.HighFrame == -1)
                highFrame = (int)animation.NumFrames;

            if (highFrame > animation.NumFrames)
            {
                // magic windup for level 6 spells appears to be the only animation w/ bugged data
                //Console.WriteLine($"MotionTable.GetAnimationLength({anim}): highFrame({highFrame}) > animation.NumFrames({animation.NumFrames})");
                highFrame = (int)animation.NumFrames;
            }

            var numFrames = highFrame - anim.LowFrame;

            return numFrames / Math.Abs(anim.Framerate); // framerates can be negative, which tells the client to play in reverse
        }

        public ACE.Entity.Position GetAnimationFinalPositionFromStart(ACE.Entity.Position position, float objScale, MotionCommand motion)
        {
            MotionStance defaultStyle = (MotionStance)DefaultStyle;

            // get the default motion for the default
            MotionCommand defaultMotion = GetDefaultMotion(defaultStyle);
            return GetAnimationFinalPositionFromStart(position, objScale, defaultMotion, defaultStyle, motion);
        }

        public ACE.Entity.Position GetAnimationFinalPositionFromStart(ACE.Entity.Position position, float objScale, MotionCommand currentMotionState, MotionStance style, MotionCommand motion)
        {
            uint motionHash = ((uint)currentMotionState & 0xFFFFFF) | ((uint)style << 16);

            if (!Links.TryGetValue(motionHash, out var link) || !link.TryGetValue((uint)motion, out var motionData))
                return position;

            var finalPosition = new ACE.Entity.Position(position);

            // loop through the animations to get our total count
            foreach (var anim in motionData.Anims)
            {
                var animation = DatManager.PortalDat.ReadFromDat<Animation>(anim.AnimId);

                var highFrame = anim.HighFrame != -1 ? anim.HighFrame : (int)animation.NumFrames;

                for (var i = anim.LowFrame; i < highFrame; i++)
                {
                    var posFrame = animation.PosFrames[i];

                    finalPosition.Pos += Vector3.Transform(posFrame.Origin, finalPosition.Rotation) * objScale;

                    finalPosition.Rotation *= posFrame.Orientation;
                }
            }
            return finalPosition;
        }
    }
}
