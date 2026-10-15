import type { BuscaRequest, BuscaResponse } from './types.ts'

export type RespostaBusca =
  | { tipo: 'sucesso'; dados: BuscaResponse }
  | { tipo: 'validacao'; erros: string[] }

export async function buscarProfessores(request: BuscaRequest): Promise<RespostaBusca> {
  const resposta = await fetch('/api/professores/busca', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(request),
  })

  if (resposta.ok) {
    return { tipo: 'sucesso', dados: await resposta.json() }
  }

  if (resposta.status === 400) {
    const corpo = await resposta.json().catch(() => null)

    if (Array.isArray(corpo?.erros)) {
      return { tipo: 'validacao', erros: corpo.erros }
    }
  }

  throw new Error(`Resposta inesperada da API: ${resposta.status}`)
}
