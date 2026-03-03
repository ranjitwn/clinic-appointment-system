import { useEffect } from "react";

interface Props {
  message: string;
  type?: "success" | "error";
  onClose: () => void;

  // if provided, popup becomes a confirmation popup
  onConfirm?: () => void;
  confirmText?: string;
  cancelText?: string;
};

export default function Popup({message, type = "success", onClose, onConfirm, confirmText = "OK", cancelText = "Cancel",}
    : Props) {
        useEffect(() => {
            // Auto-close only for normal toast popups
            if (onConfirm) return;

            const timer = setTimeout(() => { onClose(); }, 4000);
            return () => { clearTimeout(timer); };
        }, [onClose, onConfirm]);

        if (!message) return null;

    
  return (
    <div className={`popup ${type}`}>
      <span className="popup-text">{message}</span>

      {onConfirm ? (
        <div className="popup-actions">
          <button
            type="button"
            className="popup-btn"
            onClick={onConfirm}
          >
            {confirmText}
          </button>

          <button
            type="button"
            className="popup-btn secondary"
            onClick={onClose}
          >
            {cancelText}
          </button>
        </div>
      ) : (
        <button
          type="button"
          onClick={onClose}
          className="popup-close"
        >
          ×
        </button>
      )}
    </div>
  );
}
