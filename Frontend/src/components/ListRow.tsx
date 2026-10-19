import type { Product, Slot } from '@/lib/types.tsx';
import EmptyRow from './EmptyRow';
import FilledRow from '@/components/FilledRow.tsx';

interface props {
	slot: Slot;
	product: Product | undefined;
	onSelect: (product: Product) => void;
	onRemove: () => void;
}
function ListRow({ slot, product, onSelect, onRemove }: props) {
	if (!product) return <EmptyRow slot={slot} onSelect={onSelect} />;
	return <FilledRow slot={slot} product={product} onRemove={onRemove} />;
}

export default ListRow;
