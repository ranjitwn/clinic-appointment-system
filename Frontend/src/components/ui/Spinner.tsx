interface Props {
  text?: string;
  inline?: boolean;
}

export default function Spinner({ text, inline = false }: Props) {
  return (
    <div className={inline ? "spinner-wrap-inline" : "spinner-wrap"}>
      <div className={`spinner${inline ? " spinner-sm" : ""}`} role="status" aria-label={text ?? "Loading"} />
      {text && <span>{text}</span>}
    </div>
  );
}
