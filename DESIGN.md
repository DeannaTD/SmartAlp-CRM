---
name: SmartAlp CRM
description: A clear, restrained desktop workspace grounded in the SmartAlp Academy identity.
colors:
  primary: "#7f56d9"
  primary-hover: "#6941c6"
  primary-soft: "#f4effc"
  primary-selection: "#e9ddfb"
  primary-strong: "#53389e"
  ink: "#101828"
  muted: "#475467"
  secondary-text: "#667085"
  label: "#344054"
  line: "#e4e7ec"
  field-border: "#d0d5dd"
  surface: "#ffffff"
  canvas: "#f9fafb"
  danger: "#b42318"
typography:
  headline:
    fontFamily: "Inter, sans-serif"
    fontSize: "28px"
    fontWeight: 650
    lineHeight: 1.25
    letterSpacing: "-0.025em"
  title:
    fontFamily: "Inter, sans-serif"
    fontSize: "18px"
    lineHeight: 1.4
    letterSpacing: "-0.02em"
  body:
    fontFamily: "Inter, sans-serif"
    fontSize: "14px"
    lineHeight: 1.5
  table:
    fontFamily: "Inter, sans-serif"
    fontSize: "13px"
    lineHeight: 1.5
  label:
    fontFamily: "Inter, sans-serif"
    fontSize: "13px"
    fontWeight: 600
  button:
    fontFamily: "Inter, sans-serif"
    fontSize: "14px"
    fontWeight: 600
    lineHeight: 1.4
rounded:
  control: "8px"
  surface: "12px"
  dialog: "16px"
spacing:
  compact: "8px"
  small: "12px"
  control-gap: "16px"
  grid-gap: "20px"
  surface-inset: "24px"
  section-gap: "32px"
  content-inset: "36px"
components:
  button-primary:
    backgroundColor: "{colors.primary}"
    textColor: "{colors.surface}"
    typography: "{typography.button}"
    rounded: "{rounded.control}"
    padding: "10px 18px"
  button-primary-hover:
    backgroundColor: "{colors.primary-hover}"
    textColor: "{colors.surface}"
  button-secondary:
    backgroundColor: "{colors.surface}"
    textColor: "{colors.muted}"
    typography: "{typography.button}"
    rounded: "{rounded.control}"
    padding: "10px 18px"
  button-danger:
    backgroundColor: "{colors.danger}"
    textColor: "{colors.surface}"
    typography: "{typography.button}"
    rounded: "{rounded.control}"
    padding: "10px 18px"
  input:
    backgroundColor: "{colors.surface}"
    textColor: "{colors.ink}"
    typography: "{typography.body}"
    rounded: "{rounded.control}"
    padding: "10px 12px"
  nav-current:
    backgroundColor: "{colors.primary}"
    textColor: "{colors.surface}"
    rounded: "{rounded.control}"
    padding: "11px 14px"
  course-card:
    backgroundColor: "{colors.surface}"
    textColor: "{colors.ink}"
    rounded: "{rounded.surface}"
    padding: "24px"
  table:
    backgroundColor: "{colors.surface}"
    textColor: "{colors.ink}"
    typography: "{typography.table}"
    rounded: "{rounded.surface}"
---

# Design System: SmartAlp CRM

## Overview

**Creative North Star: "Clear, restrained operational interface"**

This phrase records the confirmed direction; no additional creative metaphor or visual anti-reference was confirmed. White navigation and content surfaces sit on a quiet grey canvas. Purple identifies selection and primary actions, with pale purple supporting grouped or selected content. Compact, readable forms and tables carry the working interface.

The existing SmartAlp identity is the visual authority. This system documents the implemented refresh, rather than inventing a replacement brand. Preserve the original academy mark and supplied course materials. Desktop is the primary operating context; narrow layouts keep the same workflows accessible.

**Key Characteristics:**

- White working surfaces and a quiet grey canvas.
- Purple actions and selection with restrained tonal support.
- Inter typography and compact, readable operational content.
- Original SmartAlp identity and materials.
- A desktop navigation rail with responsive access to the same work.

Implementation evidence: `NewGenBridge/wwwroot/css/workspace.css`, the shared workspace brand/location partials and layouts, and the login, course and price views. `PRODUCT.md` supplies the confirmed brand and desktop constraints.

Asset provenance recorded during the refresh: [SmartAlp Academy reference](https://smartalpacademy.com/welcome/kz/ru/), [official Inter source](https://smartalpacademy.com/fonts/Inter-VariableFont_opsz,wght.ttf), and [official logo source](https://smartalpacademy.com/img/logo-square.svg). The application self-hosts these as `NewGenBridge/wwwroot/fonts/Inter-Variable.ttf` and `NewGenBridge/wwwroot/img/smartalp-brand.svg`.

## Colors

The palette pairs academy purple with cool, low-contrast neutral surfaces; the frontmatter is the normative token inventory.

### Primary

- **Academy purple** (`primary`): primary buttons, current top-level navigation, native checkbox/radio accents and visible keyboard focus.
- **Deep academy purple** (`primary-hover`): primary hover states and action links.
- **Soft academy purple** (`primary-soft`): open navigation groups, selected sub-navigation, grouped table headings, lesson hover and login identity panel.
- **Purple selection tint** (`primary-selection`): text selection and the login panel divider.
- **Strong purple text** (`primary-strong`): selected sub-navigation and grouped content labels on pale purple.

### Neutral

- **Ink**: headings, field values and main content.
- **Muted slate**: navigation, secondary buttons and supporting paragraphs.
- **Secondary slate**: placeholders, notes and metadata.
- **Label slate**: login field labels.
- **White surface**: navigation, header, tables, cards and fields.
- **Quiet canvas**: workspace background, table headers and neutral hover states.
- **Soft divider**: surface borders and table row separators.
- **Field border**: input and secondary-button outlines.

Danger red is a semantic exception used for destructive controls and validation errors; it is not a second brand accent.

**The Functional Accent Rule.** Use strong purple for actions, selection and focus; use its pale tint to support grouping and contextual emphasis.

## Typography

**Body and interface font:** self-hosted Inter variable, with a sans-serif fallback. The font face supports weights 100–900 and uses `font-display: swap`. There is no separate display face.

The interface uses a compact functional hierarchy, rather than a fixed geometric type scale. Page headings follow `headline`; course titles follow `title`; ordinary controls and text follow `body`; tables follow `table`. Section headings use 20px at line-height 1.25. Mobile page headings reduce to 24px. The login heading is 30px, and its labels follow `label`.

Supporting navigation/account metadata ranges from 11–13px on desktop. Main supporting paragraphs have a maximum measure of 75ch and line-height 1.7. Tables and lesson numbering use tabular numerals for easier scanning.

## Layout

The desktop shell has a sticky full-height white navigation rail, an 88px white header, and an independently scrollable main area. The rail is 244px wide, with a 76px collapsed state. Main content uses the `content-inset` spacing token and is capped at 1800px.

Course collections use three equal columns with the grid-gap token; at 1600px and above they use four. At 1100px and below, the rail is 220px, content/header side insets reduce to 28px, and courses use two columns. At 760px and below, the header is 72px, a menu button opens a 260px fixed rail, content uses 24px vertical and 16px horizontal insets, and courses and form grids become single-column.

Wide tables retain a 600px minimum width inside a scrollable bordered region. The price editor fills available content height, keeps its table header sticky, and places the save action beneath the scroll region. Do not force a wide data grid to fit by shrinking its text.

Login uses two columns, with the logo on a pale-purple panel and a form capped at 360px on white. At the mobile breakpoint, the logo panel sits above the form. These are observed page-specific compositions, not a mandatory template for every screen.

## Elevation & Depth

The workspace relies on white surfaces, soft borders and background tone. Ordinary working cards and tables do not require a shadow. Modal overlays dim the page with `rgb(16 24 40 / 45%)`; the alert box uses `0 18px 48px rgb(16 24 40 / 18%)`.

**The Quiet Surface Rule.** Separate ordinary working regions with tone and borders; reserve the documented shadow for alerts.

## Shapes

Controls and navigation items use the control radius. Cards, table wrappers, settings sections and progress surfaces use the surface radius. Dialog and alert containers use the dialog radius. Dividers and field borders are 1px. The course creation tile is a dashed-outline member of the course grid; keep that distinction specific to this creation pattern.

## Components

### Buttons

Compact, clear actions. Primary and secondary controls use the frontmatter button typography, padding and radius, with a 42px minimum height. Primary buttons deepen on hover; secondary buttons have a field-coloured border and quiet-canvas hover. Destructive actions use danger red. Disabled controls reduce opacity to .55 and show a not-allowed cursor.

Background transitions last 160ms. Keyboard focus uses a 3px academy-purple outline with a 3px offset. Honour reduced motion by disabling transitions and animation durations.

### Inputs / Fields

White fields have a field border, the control radius, a 42px minimum height and a purple caret. Focus changes the border to academy purple; placeholders use secondary slate. Validation messages use danger red. Form layouts generally use 16px gaps, with 20px grid gaps.

Table fields reduce to 36px minimum height, 7px 9px padding and table-sized text. Numeric price fields use a 96px width. Login text fields are at least 46px high. Its native remember-me checkbox is 16px square, aligned with its label in a flex row with a 10px gap.

### Navigation

The white rail carries the original logo and role-dependent links. Top-level links are at least 44px high with 14px medium text; current pages are solid purple with white text. Open groups and selected child links use pale purple, with deep-purple text. Neutral hover is subtle. Active state is expressed with `aria-current="page"`; menu controls retain their expanded state and controlled-region relationship.

The header provides current section, language, account and profile context. On narrow screens, the account text and header logout shortcut hide while navigation remains available through the menu.

### Cards / Containers

Course cards use a soft border, the surface radius and surface-inset padding. Titles and descriptions remain separate; the delete action stays a separate target from opening a course. The hover border is `#b9a1e7`, with a `#fefcff` background. Progress and settings panels share the same surface family.

### Tables

Tables use a white bordered wrapper, left-aligned content, quiet-canvas headers, 13px text and 13px 16px cell padding. Row dividers provide structure; the last row has no bottom divider. Row hover uses `#fcfaff`. Numeric cells use tabular figures. Scroll regions remain keyboard focusable, with the focus outline inset by 3px.

## Do's and Don'ts

### Do:

- **Do** reuse the self-hosted Inter font and the original SmartAlp logo.
- **Do** use white working surfaces, quiet neutral dividers and purple actions or selection.
- **Do** preserve readable table density, horizontal table scrolling and visible keyboard focus.
- **Do** retain role conditions, routes, translations, field names, validation and event handlers when extending the interface.
- **Do** use real course materials and actual product data.

### Don't:

- **Don't** replace the established academy identity with invented logos or decorative imagery.
- **Don't** turn ordinary workspace cards into elevated promotional panels.
- **Don't** remove or hide required actions to simplify a layout.
- **Don't** introduce fabricated statistics or marketing claims as interface content.
