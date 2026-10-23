# Lexia: Requirements (running summary)

_Last updated: 2026-10-23. Built from an interview with Mansi Bansal. Decisions 1 to 8 and 12 are Mansi's answers; 9 to 11 and 13 use the recommended option, at Mansi's request._

## Competitor baseline

**LitPal (litpal.ai):** AI writing, illustrating, and editing for KDP books. Book types: non-fiction (chapters of about 500 to 5,000 words), illustrated children's stories, coloring books, color by number, word search, word fit, and Sudoku. Image generation and editing, cover tools, LitPal Research for niche and title ideas, PDF export. Browser only. Credit-based plans from $29.99 to $199.99 a month, with every tool on every plan and a 3-day trial.

**Book Bolt (bookbolt.io):** research and design for low-content KDP books. Niche and trend research, Amazon keyword search volumes, the KDP Spy Chrome extension, competitor tracking, a cover designer (16 sizes), an interior generator with bleed and trim handled, a listing and metadata tool, PuzzleWizz puzzles on Pro, training webinars. Plans around $9.99 and $19.99 a month. Some free tools (interior creator, category finder, PDF combiner).

**Gap to exploit:** LitPal is strong on AI content but light on market research. Book Bolt is strong on research and formatting but has no real AI writing. Neither covers publishing beyond KDP, launch marketing, or sales tracking well.

## Decisions so far

1. **Target users:** both low-content sellers (coloring, puzzles, journals) and writers of real content (kids' stories, non-fiction, fiction), in one tool.
2. **Core promise:** end to end. One guided flow from idea to niche research to AI creation to a KDP-ready file and listing.
3. **Book types at launch:** all of them. Low-content (coloring books, color by number, puzzles such as word search, sudoku, crossword, mazes, journals, planners, logbooks), illustrated kids' stories, non-fiction, and full-length fiction (chapters, characters, series tracking).
4. **Research depth:** Book Bolt level plus AI insight. Real Amazon search volume, BSR and competition data, category finder, competitor tracking, plus an AI opportunity score per niche, review mining (what readers dislike in competing books), trend alerts, and AI title and subtitle suggestions.
5. **AI writing:** both modes. One-click full draft from a prompt, and a co-writer mode (outline, then chapter by chapter with author edits). Plus a story bible (characters, settings, tone) for consistency across long books and series, and a "write in my voice" option trained on the author's samples.
6. **AI illustration:** LitPal level (themed coloring pages, kids' story illustrations, image editing) plus consistent characters across every page, one art style locked per book, in-painting to fix parts of an image, and a commercial-use license check.
7. **Cover and interior design:** Book Bolt level (all KDP trim sizes, auto spine width from page count, bleed and margins, interior generator) plus AI cover concepts from genre and title, a thumbnail preview showing the cover on an Amazon search page next to competitors, and a pre-upload checker for KDP rejection issues (margins, fonts, low-res images).
8. **Publishing and export:** KDP exports (print PDF, cover PDF, ebook) with AI listing help (description, 7 backend keywords, categories, A+ content ideas), plus IngramSpark, Draft2Digital, Apple Books, Etsy printables, and audiobook-ready scripts. Also direct publishing to KDP from inside Lexia through a browser extension. _Risk: Amazon has no public upload API, so this automation may break when KDP changes and could put user accounts at risk. Needs a careful design and clear user consent._
9. **After publishing (recommended):** sales dashboard from imported KDP royalty reports, BSR and keyword rank tracking, review monitoring, and AI suggestions when a book slips (refresh cover, add keywords), feeding back into research. Launch marketing tools (Amazon Ads keyword lists, social and ad copy, launch checklist) come in a later phase.
10. **Pricing (recommended):** hybrid. A free tier with KDP calculators, puzzle and journal generators, and limited research. Paid subscription tiers that include monthly AI credits for writing and images, so heavy AI use is paid for fairly. Every tool on every paid tier, unlike Book Bolt's Pro-only puzzles.
11. **Platform (recommended):** responsive web app first, plus a Chrome extension for Amazon research overlays and the KDP upload helper. Mobile app later.
12. **Tech stack (Mansi's choice):** React.js frontend and a .NET (latest, .NET 10) backend, built as microservices with Clean Architecture and SOLID, using only free, open source libraries and tools. Databases: PostgreSQL, Redis, and MongoDB Community for documents such as manuscripts and story bibles (recommended NoSQL; FerretDB is a fully open source drop-in). Supporting pieces (recommended defaults, not yet confirmed): Keycloak for sign-in, YARP as the API gateway, RabbitMQ for messaging, Docker Compose for local runs, self-hosted AI models (Ollama for text, Stable Diffusion for images) behind a swappable interface with an optional paid-API adapter.
13. **Launch scope (recommended):** build in phases.
    - **Phase 1 (MVP):** accounts, projects (books), guided flow shell, KDP specs engine (trim sizes, spine width, bleed, cover size), low-content generators (lined and dotted journals, word search, sudoku, mazes) with print-ready PDF export, pre-upload KDP checker, AI listing generator, AI co-writer outline and chapters. _Phase 1a (foundation) is built: everything here except the two AI features, which are Phase 1b._
    - **Phase 2:** niche research with real Amazon data and AI opportunity score, AI illustration with consistent characters, cover designer with thumbnail preview, story bible.
    - **Phase 3:** other stores, Chrome extension and KDP upload, sales and rank tracking, marketing tools, write-in-my-voice.

## Open questions

- Data source for Amazon search volume and BSR (paid API versus own scraping) needs a decision before Phase 2.
- Image model provider for illustrations (default: self-hosted Stable Diffusion).
- Object storage for PDFs and images: MinIO's free distribution changed in 2025, so SeaweedFS or Garage may be the safer open source choice.
