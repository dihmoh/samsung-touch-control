## 2024-03-17 - WinForms Accessibility and Interaction Polish
**Learning:** Even though this is a WinForms app rather than a web frontend, similar UX principles apply. Screen readers need `AccessibleName` and `AccessibleRole` on custom buttons (like the `✕` close button and the large `ON/OFF` toggle button), analogous to ARIA labels. Furthermore, interactive elements with `FlatStyle.Flat` lack default hover feedback, making them feel less responsive.
**Action:** Always add `AccessibleName` and `AccessibleRole` to critical custom controls. Provide visual feedback by configuring `FlatAppearance.MouseOverBackColor` and `FlatAppearance.MouseDownBackColor` to assure users their interactions are registered.

## 2024-05-17 - Dynamic WinForms Button Accessibility
**Learning:** In custom-styled WinForms applications, large graphical toggle buttons (like the ON/OFF button) often rely on color and short text changes to indicate state. However, screen readers read `AccessibleName` or text but miss dynamic state changes if `AccessibleDescription` is not actively updated when the button toggles.
**Action:** Always bind the `AccessibleDescription` or `AccessibleName` to update dynamically alongside visual state changes (like color/text) in `UpdateUI()` methods so screen reader users hear "Touch Enabled" or "Touch Disabled", not just "Toggle Touchscreen State" and "ON/OFF".

## 2024-05-18 - Borderless WinForms Window Navigation and Dragging
**Learning:** Custom borderless WinForms windows (Form.FormBorderStyle = None) do not inherently support standard keyboard dismissals like the `Escape` key, trapping keyboard users unless a dedicated focusable button is provided. Additionally, labels overlapping custom top bar drag zones steal the `MouseDown` event, creating frustrating "dead zones" where the window cannot be dragged.
**Action:** Explicitly implement `ProcessCmdKey` to handle the `Escape` key for graceful dismissal. Always apply drag handlers recursively or collectively to the panel and all overlapping labels to ensure a continuous and smooth dragging surface.

## 2024-05-19 - Embedded WebView2 Accessibility and Focus
**Learning:** In WebView2 embedded UIs, interactive elements like custom HTML `div`s used as buttons (e.g., hotkey or startup toggles) are inaccessible to keyboard users and screen readers by default. Relying solely on `onclick` leaves out keyboard navigation entirely. Additionally, default focus rings are often invisible against custom dark themes.
**Action:** When creating custom HTML components in WebView2, always ensure they are fully accessible by adding `role` (like `button` or `switch`), `tabindex="0"`, dynamic `aria-*` states, and an explicit `onkeydown` handler for the 'Enter' and Space keys to trigger the same events as click. Provide a visible global `*:focus-visible` CSS rule (e.g., `outline: 2px solid white;`) to guarantee focus is identifiable during keyboard navigation.
