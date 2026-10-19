import { TableCell, TableRow } from '@/components/ui/table';
import type { Product, Slot } from '@/lib/types';
import { Button } from './ui/button';
import REILogo from '@/assets/REI_idsW56ot2c_0.svg';
import { formatPrice } from '@/lib/utils.ts';

interface props {
	slot: Slot;
	product: Product;
	onRemove: () => void;
}

function FilledRow({ slot, product, onRemove }: props) {
	return (
		<TableRow>
			<TableCell className="capitalize underline">
				<a href="">{slot.label}</a>
			</TableCell>
			<TableCell>
				<div className="flex flex-row items-center gap-2">
					<img
						src={product.image}
						alt=""
						className="mix-blend-multiply"
						width="5%"
						height="2%"
					/>
					<a
						href={product.link}
						className="font-semibold hover:underline"
					>
						{product.name}
					</a>
				</div>
			</TableCell>
			<TableCell>{product.weights}</TableCell>
			<TableCell className="font-bold text-green-600">
				{formatPrice(product.priceCents)}
			</TableCell>
			<TableCell>
				<div className="flex flex-row gap-6">
					<a href={product.link}>
						<img src={REILogo} alt="REI Logo" width="50" />
					</a>
					<div className="space-x-2">
						<a href={product.link}>
							<Button className="bg-green-600">Buy</Button>
						</a>
						<Button variant="destructive" onClick={onRemove}>
							Remove
						</Button>
					</div>
				</div>
			</TableCell>
		</TableRow>
	);
}

export default FilledRow;
