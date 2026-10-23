import type { CoverDimensionsDto } from '@/services/api/types';

const DIAGRAM_WIDTH = 480;

/** A to-scale sketch of the cover: back, spine and front, with bleed and safe zones. */
export const CoverDiagram = ({ cover }: { cover: CoverDimensionsDto }) => {
  const scale = DIAGRAM_WIDTH / cover.fullWidthIn;
  const height = cover.fullHeightIn * scale;
  const bleed = cover.bleedIn * scale;
  const safe = cover.safeZoneIn * scale;
  const spine = cover.spineWidthIn * scale;
  const panel = (DIAGRAM_WIDTH - 2 * bleed - spine) / 2;
  const backX = bleed;
  const spineX = bleed + panel;
  const frontX = spineX + spine;
  const safeHeight = height - 2 * (bleed + safe);

  return (
    <svg
      viewBox={`0 0 ${DIAGRAM_WIDTH} ${height}`}
      className="w-full rounded-lg ring-1 ring-slate-200"
      role="img"
      aria-label="Cover layout with back cover, spine and front cover"
    >
      <rect width={DIAGRAM_WIDTH} height={height} fill="#fee2e2" />
      <rect x={bleed} y={bleed} width={DIAGRAM_WIDTH - 2 * bleed} height={height - 2 * bleed} fill="#fff" />
      <rect x={backX + safe} y={bleed + safe} width={panel - 2 * safe} height={safeHeight} fill="#eef2ff" />
      <rect x={frontX + safe} y={bleed + safe} width={panel - 2 * safe} height={safeHeight} fill="#eef2ff" />
      <rect x={spineX} y={bleed} width={Math.max(spine, 1)} height={height - 2 * bleed} fill="#a5b4fc" />
      <text x={backX + panel / 2} y={height / 2} textAnchor="middle" fontSize="13" fill="#64748b">
        Back cover
      </text>
      <text x={frontX + panel / 2} y={height / 2} textAnchor="middle" fontSize="13" fill="#64748b">
        Front cover
      </text>
      <text x={6} y={12} fontSize="9" fill="#dc2626">
        Bleed, trimmed off
      </text>
    </svg>
  );
};
