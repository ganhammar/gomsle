import { useState } from 'react';
import styled from 'styled-components';

interface TextInputStyleProps {
  isFocused: boolean;
  hasValue: boolean;
  hasError: boolean;
}

const Fieldset = styled.fieldset<TextInputStyleProps>`
  border: none;
  position: relative;
  height: 3.2rem;
  margin-bottom: 1rem;
  &:after {
    content: '';
    height: 3px;
    width: 0;
    position: absolute;
    bottom: 0;
    left: 0;
    background-color: ${({ theme: { palette }, hasError }) => hasError
      ? palette.warning.main
      : palette.primary.main};
    z-index: 1;
    transition: width 0.5s, background-color 0.5s;
  }
  ${({ isFocused }) =>
    isFocused &&
    `
    &:after {
      width: 100%;
    }
  `}
`;
const Label = styled.label<TextInputStyleProps>`
  position: absolute;
  left: 4px;
  top: 24px;
  transition: top 0.5s, font-size 0.5s;
  ${({ isFocused, hasValue }) =>
    (isFocused || hasValue) &&
    `
    top: 0;
    font-size: 0.8rem;
  `}
`;
const Input = styled.input<TextInputStyleProps>`
  border: none;
  border-bottom: 1px solid ${({ theme: { palette }, hasError }) => hasError
    ? palette.warning.main
    : palette.divider.main};
  background: none;
  padding: 0 4px 2px 4px;
  width: 100%;
  height: 2.1rem;
  margin: 1.1rem 0 0 0;
  transition: border-bottom-color 0.5s;
  &:focus {
    outline: none;
  }
`;

interface Props {
  type: 'text' | 'password';
  value: string;
  title: string;
  onChange: (value: string) => void;
  isDisabled?: boolean;
  hasError?: boolean;
}

export default function TextInput({
  type,
  value,
  title,
  onChange,
  isDisabled,
  hasError,
}: Props) {
  const [isFocused, setIsFocused] = useState(false);

  return (
    <Fieldset
      isFocused={isFocused}
      hasValue={Boolean(value)}
      hasError={hasError ?? false}
    >
      <Label
        isFocused={isFocused}
        hasValue={Boolean(value)}
        hasError={hasError ?? false}
      >
        {title}
      </Label>
      <Input
        type={type}
        value={value}
        onChange={({ target: { value } }) => onChange(value)}
        disabled={isDisabled}
        onFocus={() => setIsFocused(true)}
        onBlur={() => setIsFocused(false)}
        isFocused={isFocused}
        hasValue={Boolean(value)}
        hasError={hasError ?? false}
      />
    </Fieldset>
  );
}
