import Decimal from "decimal.js";

const ALIQUOTAS_INSS = [
  { de: 0, ate: 1412.00, aliquota: 0.075 },
  { de: 1412.01, ate: 2666.68, aliquota: 0.09 },
  { de: 2666.69, ate: 4000.03, aliquota: 0.12 },
  { de: 4000.04, ate: 7786.02, aliquota: 0.14 },
];

const ALIQUOTAS_IRRF = [
  { de: 0, ate: 2259.20, aliquota: 0, deducao: 0 },
  { de: 2259.21, ate: 2826.65, aliquota: 0.075, deducao: 169.44 },
  { de: 2826.65, ate: 3751.06, aliquota: 0.15, deducao: 381.44 },
  { de: 3751.06, ate: 4664.68, aliquota: 0.225, deducao: 662.77 },
  { de: 4664.69, ate: 99999999999, aliquota: 0.275, deducao: 896.00 }
];

export function calcularINSS(salario) {
  let totalINSS = new Decimal(0);

  // Encontra a última faixa de salário
  const ultimaFaixa = ALIQUOTAS_INSS[ALIQUOTAS_INSS.length - 1];

  // Limita o salário ao teto da última faixa, se necessário
  salario = new Decimal(Math.min(salario, ultimaFaixa.ate));

  // Loop para calcular o INSS para cada faixa
  for (let i = 0; i < ALIQUOTAS_INSS.length; i++) {
    const faixa = ALIQUOTAS_INSS[i];
    // Calcula o valor da faixa atual
    let valorFaixa = new Decimal(Math.min(salario, faixa.ate)).minus(i === 0 ? 0 : ALIQUOTAS_INSS[i - 1].ate);
    totalINSS = totalINSS.plus(valorFaixa.times(faixa.aliquota));

    // Se o salário for menor que a próxima faixa, interrompe o loop
    if (salario.lte(faixa.ate)) break;
  }

  totalINSS = totalINSS.toDecimalPlaces(2, Decimal.ROUND_DOWN);

  return totalINSS.toFixed(2);
}

export function calcularIRRF(base, parcela_inss) {
  base = new Decimal(base).minus(parcela_inss);
  const faixa = ALIQUOTAS_IRRF.filter((a) => base.gte(a.de) && base.lte(a.ate))[0];
  const totalIRRF = new Decimal(base).times(faixa.aliquota).minus(faixa.deducao).toDecimalPlaces(2, Decimal.ROUND_DOWN);
  return totalIRRF.toFixed(2);
}

export function calcularHorasExtras(valorHoraTrabalho, totalHorasExtras, acrescimo) {
  const valorHoraExtras = new Decimal(valorHoraTrabalho).times(acrescimo);
  const valorTotalHE = new Decimal(totalHorasExtras).times(valorHoraExtras);
  return valorTotalHE.toFixed(2);
}

export function calcularDSRHorasExtras(valorTotalHE, diasUteis, domingosEFeriados) {
  const valorTotalDSR = new Decimal(valorTotalHE).dividedBy(diasUteis).times(domingosEFeriados);
  return valorTotalDSR.toFixed(2);
}

export function calcularAdicionalNoturno(totalHorasNoturnas, valorAdicionalNoturno) {
  const valorTotalAdicionalNoturno = new Decimal(totalHorasNoturnas).times(valorAdicionalNoturno);
  return valorTotalAdicionalNoturno.toDecimalPlaces(2, Decimal.ROUND_DOWN).toFixed(2);
}

export function calcularHoraTrabalho(salario) {
  return new Decimal(salario).dividedBy(200).toFixed(2);
}