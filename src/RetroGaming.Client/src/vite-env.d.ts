/// <reference types="vite/client" />

interface ImportMetaEnv {
  /** Base URL for the backend API (defaults to the relative "/api" proxy). */
  readonly VITE_API_URL?: string;
  /** Map tile URL template ({z}/{x}/{y}, optional {s}). Defaults to OpenStreetMap. */
  readonly VITE_MAP_TILE_URL?: string;
  /** API key for a keyed tile provider (omit for plain OpenStreetMap). */
  readonly VITE_MAP_API_KEY?: string;
  /** Query-parameter name used for the tile API key (default "apiKey"). */
  readonly VITE_MAP_API_KEY_PARAM?: string;
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}
