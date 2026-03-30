## 2024-03-17 - WinForms Accessibility and Interaction Polish
**Learning:** Even though this is a WinForms app rather than a web frontend, similar UX principles apply. Screen readers need `AccessibleName` and `AccessibleRole` on custom buttons (like the `✕` close button and the large `ON/OFF` toggle button), analogous to ARIA labels. Furthermore, interactive elements with `FlatStyle.Flat` lack default hover feedback, making them feel less responsive.
**Action:** Always add `AccessibleName` and `AccessibleRole` to critical custom controls. Provide visual feedback by configuring `FlatAppearance.MouseOverBackColor` and `FlatAppearance.MouseDownBackColor` to assure users their interactions are registered.

## 2024-05-17 - Dynamic WinForms Button Accessibility
**Learning:** In custom-styled WinForms applications, large graphical toggle buttons (like the ON/OFF button) often rely on color and short text changes to indicate state. However, screen readers read `AccessibleName` or text but miss dynamic state changes if `AccessibleDescription` is not actively updated when the button toggles.
**Action:** Always bind the `AccessibleDescription` or `AccessibleName` to update dynamically alongside visual state changes (like color/text) in `UpdateUI()` methods so screen reader users hear "Touch Enabled" or "Touch Disabled", not just "Toggle Touchscreen State" and "ON/OFF".

## 2024-05-18 - Borderless WinForms Window Navigation and Dragging
**Learning:** Custom borderless WinForms windows (Form.FormBorderStyle = None) do not inherently support standard keyboard dismissals like the `Escape` key, trapping keyboard users unless a dedicated focusable button is provided. Additionally, labels overlapping custom top bar drag zones steal the `MouseDown` event, creating frustrating "dead zones" where the window cannot be dragged.
**Action:** Explicitly implement `ProcessCmdKey` to handle the `Escape` key for graceful dismissal. Always apply drag handlers recursively or collectively to the panel and all overlapping labels to ensure a continuous and smooth dragging surface.

## 2024-05-19 - WebView2 Embedded HTML Accessibility
**Learning:** Even when UI components are built using HTML/CSS within a desktop application via WebView2 (instead of native WinForms controls), they must still adhere to web accessibility standards. Elements like custom interactive `div`s lack default keyboard focus and activation behavior.
**Action:** Always ensure that interactive embedded HTML elements receive `role="button"`, `tabindex="0"`, and `onkeydown` handlers (listening for 'Enter' or 'Space' and calling `event.preventDefault()` to stop unwanted scrolling). Use `:focus-visible` styling to provide clear visual feedback during keyboard navigation.
